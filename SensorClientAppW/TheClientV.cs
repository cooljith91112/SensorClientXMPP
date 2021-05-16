using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Waher.Networking.XMPP.Sensor;
using Waher.Networking.XMPP;
using Waher.Things;
using Waher.Networking.XMPP.ServiceDiscovery;
using Waher.Networking.XMPP.Provisioning;
using System.Windows.Forms;
using Waher.Things.SensorData;
using Waher.Runtime.Settings;
using Waher.Persistence.Files;
using Waher.Persistence;
using System.IO;
using Waher.Runtime.Inventory;
using System.Reflection;
using Waher.Persistence.Serialization;
using Waher.Content.Xml;
using Timer = System.Threading.Timer;
using Waher.Networking.XMPP.Provisioning.SearchOperators;

namespace SensorClientAppW
{
    class TheClientV
    {
        string Key = "Your_Broker_Key";
        string Secret = "Your_Broker_Secret";
        private XmppClient xmppClient = null;
        string Host = "1451.ieeehyd.org";
        int Port = 5222;
        string UserName = "demoControllApp2";
        string PasswordHash = "123456";
        private Form1 formW = null;
        private SensorClient sensorClient;
        private string sensorJid = null;
        private ThingReference sensor = null;
        private SensorDataSubscriptionRequest subscription = null;
        private double? pressure = null;
        private double? humidity = null;
        private string deviceId = null;
        private FilesProvider db = null;
        private ThingRegistryClient registryClient = null;
        private bool Location = false;
        private string COUNTRY = "IN";
        private string REGION = "TVM";
        private string CITY = "TVM";
        private string AREA = "area";
        private string STREET = "Street";
        private string STREETNR = "StreetNr";
        private string BLD = "Third";
        private string APT = "1";
        private string ROOM = "room1";
        private string NAME = "Kochi";
        private DateTime lastFindFriends = DateTime.MinValue;
        public TheClientV(string Host, int Port, string UserName, string Password, Form1 formW)
        {
            this.Host = Host;
            this.Port = Port;
            this.UserName = UserName;
            this.PasswordHash = Password;
            this.formW = formW;
            this.init();
        }

        async void init()
        {
            Types.Initialize(
                typeof(FilesProvider).GetTypeInfo().Assembly,
                typeof(ObjectSerializer).GetTypeInfo().Assembly,
                typeof(RuntimeSettings).GetTypeInfo().Assembly,
                typeof(Waher.Content.IContentEncoder).GetTypeInfo().Assembly,
                typeof(XmppClient).GetTypeInfo().Assembly,
                typeof(Waher.Content.Markdown.MarkdownDocument).GetTypeInfo().Assembly,
                typeof(XML).GetTypeInfo().Assembly,
                typeof(Waher.Script.Expression).GetTypeInfo().Assembly,
                typeof(Waher.Script.Graphs.Graph).GetTypeInfo().Assembly,
                typeof(Waher.Script.Persistence.SQL.Select).GetTypeInfo().Assembly,
                typeof(TheClientV).Assembly);
            db = await FilesProvider.CreateAsync(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    Path.DirectorySeparatorChar + "IoT-DEMO-APP", "Default", 8192, 1000, 8192, Encoding.UTF8, 10000);
            Database.Register(db);

            await db.RepairIfInproperShutdown(null);
            await db.Start();
            this.deviceId = await RuntimeSettings.GetAsync("DeviceId", string.Empty);
            if (string.IsNullOrEmpty(this.deviceId))
            {
                this.deviceId = Guid.NewGuid().ToString().Replace("-", string.Empty);
                await RuntimeSettings.SetAsync("DeviceId", this.deviceId);
            }

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    Path.DirectorySeparatorChar + "IoT-DEMO-APP");
            Console.WriteLine(this.deviceId);

            this.xmppClient = new XmppClient(this.Host, this.Port, this.UserName, this.PasswordHash, "en", typeof(TheClientV).Assembly);
            this.xmppClient.AllowRegistration(Key, Secret);
            this.xmppClient.OnConnectionError += this.ConnectionError;
            this.xmppClient.OnStateChanged += this.OnStateChanged;
            this.xmppClient.OnRosterItemAdded += OnRosterItemAdded;
            this.xmppClient.OnRosterItemUpdated += OnRosterItemUpdated;
            this.xmppClient.OnRosterItemRemoved += OnRosterItemRemoved;
            this.xmppClient.Connect();
        }

        private Task ConnectionError(object _, Exception ex)
        {
            Console.WriteLine(ex);
            return Task.CompletedTask;
        }

        private async Task OnStateChanged(object Sender, XmppState State)
        {
            switch (State)
            {
                case XmppState.Connected:
                    Console.WriteLine("Connected");
                    this.formW.disableFormGroup();
                    this.formW.setConnectionStatus(true);
                    this.AddFeatures();
                    await this.RegisterDevice();
                    //this.formW.su
                    //this.FindSensors();
                    break;
                case XmppState.Error: 
                    this.formW.setConnectionStatus(false); 
                    Console.WriteLine("Connection Error"); 
                    break;
                case XmppState.Offline:
                    this.formW.setConnectionStatus(false);
                    Console.WriteLine("Connection Error"); 
                    break;
            }
        }

        private Task OnRosterItemAdded(object obj, RosterItem Item)
        {
            Console.Write("Roster Item Added " + Item.BareJid);
            if (Item.IsInGroup("Sensor"))
            if (Item.IsInGroup("Sensor"))
            {
                Console.WriteLine("Requesting presence subscription."+Item.BareJid);
                this.xmppClient.RequestPresenceSubscription(Item.BareJid);
            }
            return Task.CompletedTask;
        }

        private Task OnRosterItemUpdated(object obj, RosterItem Item)
        {
            Console.Write("Roster Item Updated " + Item.BareJid);
            bool IsSensor;
            if ((IsSensor = (this.sensorJid != null && string.Compare(Item.BareJid, this.sensorJid, true) == 0)) &&
                (Item.State == SubscriptionState.None || Item.State == SubscriptionState.From) &&
                Item.PendingSubscription != PendingSubscription.Subscribe)
            {
                this.FriendshipLost(Item);
            }
            else if (IsSensor)
                this.SubscribeToSensorData();
            return Task.CompletedTask;
        }
        private void FriendshipLost(RosterItem Item)
        {
            bool UpdateRegistration = false;

            if (string.Compare(Item.BareJid, this.sensorJid, true) == 0)
            {
                this.sensorJid = null;
                this.sensor = null;
                UpdateRegistration = true;
            }

            if (UpdateRegistration)
                Task.Run(this.RegisterDevice);
        }

        private Task OnRosterItemRemoved(object obj, RosterItem Item)
        {
            Console.Write("Roster Item Removed " + Item.BareJid);
            this.FriendshipLost(Item);
            return Task.CompletedTask;
        }

        private async Task RegisterDevice()
        {
            string ThingRegistryJid = await RuntimeSettings.GetAsync("ThingRegistry.JID", string.Empty);

            if (!string.IsNullOrEmpty(ThingRegistryJid))
            {
                Console.WriteLine("Things Already Registered");
                await this.RegisterDevice(ThingRegistryJid);
            }
            else
            {
                Console.Write("Searching for Thing Registry.");

                this.xmppClient.SendServiceItemsDiscoveryRequest(this.xmppClient.Domain, (sender, e) =>
                {
                    foreach (Item Item in e.Items)
                    {
                        this.xmppClient.SendServiceDiscoveryRequest(Item.JID, async (sender2, e2) =>
                        {
                            try
                            {
                                Item Item2 = (Item)e2.State;

                                if (e2.HasFeature(ThingRegistryClient.NamespaceDiscovery))
                                {
                                    Console.WriteLine("Thing registry found.: " + Item2.JID);

                                    await RuntimeSettings.SetAsync("ThingRegistry.JID", Item2.JID);
                                    await this.RegisterDevice(Item2.JID);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString(), "Error Accessing Things Registry");
                            }
                        }, Item);
                    }

                    return Task.CompletedTask;

                }, null);
            }
        }

        private async Task RegisterDevice(string RegistryJid)
        {
            if (this.registryClient is null || this.registryClient.ThingRegistryAddress != RegistryJid)
            {
                if (this.registryClient != null)
                {
                    this.registryClient.Dispose();
                    this.registryClient = null;
                }

                this.registryClient = new ThingRegistryClient(this.xmppClient, RegistryJid);
            }

            string s;
            List<MetaDataTag> MetaInfo = new List<MetaDataTag>()
            {
                new MetaDataStringTag("CLASS", "Controller"),
                new MetaDataStringTag("TYPE", "Controller"),
                new MetaDataStringTag("MAN", "DEMO"),
                new MetaDataStringTag("MODEL", "IOT DEMO Controller"),
                new MetaDataStringTag("SN", this.deviceId),
                new MetaDataNumericTag("V", 1.0)
            };

            MetaInfo.Add(new MetaDataStringTag("COUNTRY", COUNTRY));
            MetaInfo.Add(new MetaDataStringTag("REGION", REGION));
            MetaInfo.Add(new MetaDataStringTag("CITY", CITY));
            MetaInfo.Add(new MetaDataStringTag("AREA", AREA));
            MetaInfo.Add(new MetaDataStringTag("STREET", STREET));
            MetaInfo.Add(new MetaDataStringTag("STREETNR", STREETNR));
            MetaInfo.Add(new MetaDataStringTag("BLD", BLD));
            MetaInfo.Add(new MetaDataStringTag("APT", APT));
            MetaInfo.Add(new MetaDataStringTag("ROOM", ROOM));
            MetaInfo.Add(new MetaDataStringTag("NAME", NAME));

            this.UpdateRegistration(MetaInfo.ToArray());          
        }
        private void RegisterDevice(MetaDataTag[] MetaInfo)
        {
            Console.WriteLine("Registering device.");

            this.registryClient.RegisterThing(true, MetaInfo, async (sender, e) =>
            {
                try
                {
                    if (e.Ok)
                    {
                        Console.WriteLine("Registration successful.");

                        await RuntimeSettings.SetAsync("ThingRegistry.Location", true);
                        this.FindFriends(MetaInfo);
                    }
                    else
                    {
                        Console.WriteLine("Registration failed.");
                        await this.RegisterDevice();
                    }
                }
                catch (Exception ex)
                {
                   MessageBox.Show(ex.ToString(), "Error While Registering");
                }
            }, null);
        }

        private void UpdateRegistration(MetaDataTag[] MetaInfo)
        {
            Console.Write("Updating registration of device.");

            this.registryClient.UpdateThing(MetaInfo, (sender, e) =>
            {
                if (e.Ok)
                    Console.WriteLine("Registration update successful.");
                else
                {
                    Console.WriteLine("Registration update failed.");
                    this.RegisterDevice(MetaInfo);
                }

                this.FindFriends(MetaInfo);

                return Task.CompletedTask;

            }, null);
        }
        private void FindFriends(MetaDataTag[] MetaInfo)
        {
            double ms = (DateTime.Now - lastFindFriends).TotalMilliseconds;
            if (ms < 60000)
            {
                int msi = (int)Math.Ceiling(60000 - ms);
                Timer Timer = null;

                Console.WriteLine("Delaying search " + msi.ToString() + " ms.");

                Timer = new Timer((P) =>
                {
                    try
                    {
                        Timer?.Dispose();
                        this.FindFriends(MetaInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }, null, msi, Timeout.Infinite);

                return;
            }

            this.lastFindFriends = DateTime.Now;
            this.sensorJid = null;
            this.sensor = null;

            foreach (RosterItem Item in this.xmppClient.Roster)
            {
                if (Item.IsInGroup("Sensors"))
                {
                    this.sensorJid = Item.BareJid;
                    this.sensor = this.GetReference(Item, "Sensor");
                }
            }

            if (!string.IsNullOrEmpty(this.sensorJid))
                this.SubscribeToSensorData();

            if (string.IsNullOrEmpty(this.sensorJid))
            {
                List<SearchOperator> Search = new List<SearchOperator>();

                foreach (MetaDataTag Tag in MetaInfo)
                {
                    if (Tag is MetaDataStringTag StringTag)
                    {
                        switch (StringTag.Name)
                        {
                            case "COUNTRY":
                            case "REGION":
                            case "CITY":
                            case "AREA":
                            case "STREET":
                            case "STREETNR":
                            case "BLD":
                            case "APT":
                            case "ROOM":
                            case "NAME":
                                //Search.Add(new StringTagEqualTo(StringTag.Name, StringTag.StringValue));
                                break;
                        }
                    }
                }

                Search.Add(new StringTagEqualTo("TYPE", "Open Weather Map"));                //Search.Add(new StringTagEqualTo("TYPE", "Sensors"));

                Console.WriteLine("Searching for IOT devices in my vicinity.");

                this.registryClient.Search(0, 100, Search.ToArray(), (sender, e) =>
                {
                    Console.WriteLine(e.Things.Length.ToString() + (e.More ? "+" : string.Empty) + " things found.");

                    foreach (SearchResultThing Thing in e.Things)
                    {
                        foreach (MetaDataTag Tag in Thing.Tags)
                        {
                            if (Tag.Name == "TYPE" && Tag is MetaDataStringTag StringTag)
                            {
                                Console.WriteLine(Tag.StringValue);
                                switch (Tag.StringValue)
                                {
                                    case "Open Weather Map":
                                        if (string.IsNullOrEmpty(this.sensorJid))
                                        {
                                            this.sensorJid = Thing.Jid;
                                            this.sensor = Thing.Node;

                                            RosterItem Item = this.xmppClient[this.sensorJid];
                                            if (Item != null)
                                            {
                                                this.xmppClient.UpdateRosterItem(this.sensorJid, Item.Name,
                                                    this.AddReference(Item.Groups, "Sensor", Thing.Node));

                                                if (Item.State != SubscriptionState.Both && Item.State != SubscriptionState.To)
                                                    this.xmppClient.RequestPresenceSubscription(this.sensorJid);
                                            }
                                            else
                                            {
                                                this.xmppClient.AddRosterItem(new RosterItem(this.sensorJid, string.Empty,
                                                    this.AddReference(null, "Sensor", Thing.Node)));

                                                this.xmppClient.RequestPresenceSubscription(this.sensorJid);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    return Task.CompletedTask;

                }, null);
            }
        }
        private string[] AddReference(string[] Groups, string Prefix, IThingReference NodeReference)
        {
            List<string> Result = new List<string>()
            {
                Prefix
            };

            if (!string.IsNullOrEmpty(NodeReference.NodeId))
                Result.Add(Prefix + ".nid:" + NodeReference.NodeId);

            if (!string.IsNullOrEmpty(NodeReference.SourceId))
                Result.Add(Prefix + ".sid:" + NodeReference.SourceId);

            if (!string.IsNullOrEmpty(NodeReference.Partition))
                Result.Add(Prefix + ".prt:" + NodeReference.Partition);

            if (Groups != null)
            {
                foreach (string Group in Groups)
                {
                    if (!Group.StartsWith(Prefix))
                        Result.Add(Group);
                }
            }

            return Result.ToArray();
        }

        private ThingReference GetReference(RosterItem Item, string Prefix)
        {
            string NodeId = string.Empty;
            string SourceId = string.Empty;
            string Partition = string.Empty;

            Prefix += ".";

            foreach (string Group in Item.Groups)
            {
                if (Group.StartsWith(Prefix))
                {
                    string s = Group.Substring(Prefix.Length);
                    int i = s.IndexOf(':');
                    if (i < 0)
                        continue;

                    switch (s.Substring(0, i).ToLower())
                    {
                        case "nid":
                            NodeId = s.Substring(i + 1);
                            break;

                        case "sid":
                            SourceId = s.Substring(i + 1);
                            break;

                        case "prt":
                            Partition = s.Substring(i + 1);
                            break;
                    }
                }
            }

            return new ThingReference(NodeId, SourceId, Partition);
        }

        private void AddFeatures()
        {
            this.xmppClient.OnError += (Sender, ex) =>
            {
                Console.WriteLine("Error ", ex);
                return Task.CompletedTask;
            };

            this.xmppClient.OnPasswordChanged += (Sender, e) =>
            {
                Console.WriteLine("Password changed.", this.xmppClient.BareJID);
            };

            this.xmppClient.OnPresenceSubscribe += (Sender, e) =>
            {
                Console.WriteLine("Accepting friendship request.", this.xmppClient.BareJID, e.From);
                e.Accept();
                return Task.CompletedTask;
            };

            this.xmppClient.OnPresenceUnsubscribe += (Sender, e) =>
            {
                Console.WriteLine("Friendship removed.", this.xmppClient.BareJID, e.From);
                e.Accept();
                return Task.CompletedTask;
            };

            this.xmppClient.OnPresenceSubscribed += (Sender, e) =>
            {
                Console.WriteLine("Friendship request accepted. " + this.xmppClient.BareJID + " " + e.From);

                if (string.Compare(e.FromBareJID, this.sensorJid, true) == 0)
                    this.SubscribeToSensorData();

                return Task.CompletedTask;
            };

            this.xmppClient.OnPresenceUnsubscribed += (Sender, e) =>
            {
                Console.WriteLine("Friendship removal accepted.", this.xmppClient.BareJID, e.From);
                return Task.CompletedTask;
            };

            this.xmppClient.OnPresence += XmppClient_OnPresence;

            this.sensorClient = new SensorClient(this.xmppClient);
        }

        private Task XmppClient_OnPresence(object Sender, PresenceEventArgs e)
        {
            Console.WriteLine("Presence received." + e.Availability.ToString() + e.From);

            if (this.sensorJid != null &&
                string.Compare(e.FromBareJID, this.sensorJid, true) == 0 &&
                e.IsOnline)
            {
                this.SubscribeToSensorData();
            }
            return Task.CompletedTask;
        }

        private void SubscribeToSensorData()
        {
            RosterItem SensorItem;

            if (!string.IsNullOrEmpty(this.sensorJid) &&
                (SensorItem = this.xmppClient[this.sensorJid]) != null)
            {
                if (SensorItem.HasLastPresence && SensorItem.LastPresence.IsOnline)
                {
                    ThingReference[] Nodes;

                    if (this.sensor.IsEmpty)
                        Nodes = null;
                    else
                        Nodes = new ThingReference[] { this.sensor };

                    if (this.subscription != null)
                    {
                        this.subscription.Unsubscribe();
                        this.subscription = null;
                    }

                    Console.WriteLine("Subscribing to events. : "+ SensorItem.LastPresenceFullJid);

                    this.subscription = this.sensorClient.Subscribe(SensorItem.LastPresenceFullJid,
                        Nodes, FieldType.Momentary, new FieldSubscriptionRule[]
                        {
                            new FieldSubscriptionRule("Humidity", this.humidity, 1),
                            new FieldSubscriptionRule("Pressure", this.pressure , 1),
                        },
                        new Waher.Content.Duration(false, 0, 0, 0, 0, 0, 1),
                        new Waher.Content.Duration(false, 0, 0, 0, 0, 1, 0), true);

                    this.subscription.OnStateChanged += Subscription_OnStateChanged;
                    this.subscription.OnFieldsReceived += Subscription_OnFieldsReceived;
                    this.subscription.OnErrorsReceived += Subscription_OnErrorsReceived;
                }
                else if (SensorItem.State == SubscriptionState.From || SensorItem.State == SubscriptionState.None)
                {
                    Console.Write("Requesting presence subscription. : "+this.sensorJid);
                    this.xmppClient.RequestPresenceSubscription(this.sensorJid);
                }
            }
        }

        private Task Subscription_OnStateChanged(object obj, SensorDataReadoutState NewState)
        {
            Console.WriteLine("Sensor subscription state changed." + NewState.ToString());
            return Task.CompletedTask;
        }

        private Task Subscription_OnFieldsReceived(object obj, IEnumerable<Field> NewFields)
        {
            Console.WriteLine("Fields received");

            foreach (Field Field in NewFields)
            {
                switch (Field.Name)
                {
                    case "Humidity":
                        if (Field is QuantityField T)
                        {                          
                            this.humidity = T.Value;
                        } 
                        break;
                    case "Pressure":
                        if (Field is QuantityField P)
                        {
                            this.pressure = P.Value;
                        }
                        break;
                }
            }

            Console.WriteLine(this.humidity + "," + this.pressure);
            this.formW.UpdateDataGrid(new WeatherData() { Pressure = (double)this.pressure, Humidity = (double)this.humidity });
            return Task.CompletedTask;
        }

        private Task Subscription_OnErrorsReceived(object obj, IEnumerable<ThingError> NewErrors)
        {
            MessageBox.Show("Error Subscribing");

            return Task.CompletedTask;
        }

        public void Shutdown()
        {
            this.db?.Stop()?.Wait();
            this.db?.Flush()?.Wait();
        }

    }
}
