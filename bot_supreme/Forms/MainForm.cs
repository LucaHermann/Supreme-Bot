using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace bot_supreme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           
            FixBrowserVersion();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            ProductList = new List<ProductInfo>();
            cbCountry.SelectedIndex = 0;
            cbType.SelectedIndex = 0;
            cbExpMonth.SelectedIndex = 0;
            cbExpYear.SelectedIndex = 0;
        }

        private bool _start;
        private bool ready = false;
        private byte[] key = { 1, 9, 7, 5, 5, 7, 9, 1 };
        private List<SolvedCaptcha> solvedCaptchas;
        private UserPreference _userPreference;
        private Thread serverThread;
        private bool SolverOpened = false;
        private Thread captchaSolverThread;
        private DateTime StartingTime;
        public Form1(bool start,bool debug)
        {
            _start = start;
            InitializeComponent();
            solvedCaptchas = new List<SolvedCaptcha>();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            ProductList = new List<ProductInfo>();
            cbCountry.SelectedIndex = 0;
            cbType.SelectedIndex = 0;
            cbExpMonth.SelectedIndex = 0;
            cbExpYear.SelectedIndex = 0;
            #if DEBUG
                        this.Size = new Size(1192, 576);
            #endif
            if (debug)
            {
                this.Size = new Size(1192, 588);
            }
        }
        private  int listenPort = 11000;

        private  void StartListener()
        {
            bool done = false;
            UdpClient listener = null;

            try
            {
                listener = new UdpClient(listenPort);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
                while (!done)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    solvedCaptchas.Add(new SolvedCaptcha(Encoding.ASCII.GetString(bytes, 0, bytes.Length)));
                }

            }
            catch (Exception e)
            {
              //MessageBox.Show(@"An error has occurred during the listening!" + Environment.NewLine + e.Message,"",MessageBoxButtons.OK,MessageBoxIcon.Error);
                listenPort++;
            }
            finally
            {
               
                try
                {            
                    listener.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }  
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {


            FixBrowserVersion();
            webBrowser1.Navigate("http://www.supremenewyork.com/shop/all/", null, null,
                    "User-Agent: Mozilla/5.0 (Linux; Android 5.1.1; Nexus 5 Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/43.0.2357.65 Mobile Safari/537.36");
            WebBrowserHelper.ClearCache();
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true);
            RegKey.SetValue("Display Inline Images", "no");
            LoadSettings();
            webBrowser1.DocumentCompleted += OnCompleted;


        }
        private  List<ProductInfo> ProductList = null;
        //Code Erreur : 11
        private void FixBrowserVersion()
        {
            try
            {
                int BrowserVer, RegVal;

                // get the installed IE version
                using (WebBrowser Wb = new WebBrowser())
                    BrowserVer = Wb.Version.Major;

                // set the appropriate IE version
                if (BrowserVer >= 11)
                    RegVal = 11001;
                else if (BrowserVer == 10)
                    RegVal = 10001;
                else if (BrowserVer == 9)
                    RegVal = 9999;
                else if (BrowserVer == 8)
                    RegVal = 8888;
                else
                    RegVal = 7000;

                // set the actual key
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord);
                Key.Close();
            }
            catch (Exception e)
            {
              //  MessageBox.Show(e.Message,@"An error has been occurred!\n  CODE: 11",MessageBoxButtons.OK);
            }
          
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void WaitForBrowserComplete()
        {
            while (webBrowser1.IsBusy)
                Application.DoEvents();
            ready = false;
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();



        }

        public void OnCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ready = true;
        }

        public System.Windows.Forms.HtmlDocument GetHtmlDocument(string html)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentText = html;
            browser.Document.OpenNew(true);
            browser.Document.Write(html);
            return browser.Document;
        }
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public static void RefreshIESettings(string strProxy)
        {
            try
            {
                const int INTERNET_OPTION_PROXY = 38;
                const int INTERNET_OPEN_TYPE_PROXY = 3;

                Struct_INTERNET_PROXY_INFO struct_IPI;

                // Filling in structure 
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
                struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

                // Allocating memory 
                IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));

                // Converting structure to IntPtr 
                Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

                bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
            }
            catch (Exception ex)
            {

                //TB.ErrorLog(ex);
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {

           if( SaveSettings())
            {
                MessageBox.Show(
                    @"The purchase has been scheduled for " + dateTimePicker1.Value.ToString(CultureInfo.InvariantCulture) +
                    @"
If an error occurs during purchase, the bot will automatically retry", "", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }


        }
        async Task PutTaskDelay(int duration)
        {
            await Task.Delay(duration);
        }
       private async Task Work()
        {

           

            try
            {



                foreach (var product in ProductList)
                {
                    if (!findArticle(product.Category, product.Name, product.Color)) continue;

                    WaitForBrowserComplete();
                    if (webBrowser1.Document.GetElementsByTagName("input")["commit"] == null) continue;

                    if (webBrowser1.Document.GetElementById("size") != null)
                    {
                        if (webBrowser1.Document.GetElementById("size").Children.Count != 0)
                        {
                            var size_finded = false;

                            var size_available = webBrowser1.Document.GetElementById("size").Children;
                            foreach (HtmlElement size in size_available)
                            {
                                if (size.InnerText != product.Size) continue;
                                else
                                {
                                    webBrowser1.Document.GetElementById("size").SetAttribute("value", size.GetAttribute("value"));
                                    size_finded = true;
                                    break;
                                }
                                

                            }

                            if (!size_finded)
                            {
                                foreach (HtmlElement size in size_available)
                                {
                                    if (size.InnerText == product.SizeDefault)
                                        webBrowser1.Document.GetElementById("size").SetAttribute("value", size.GetAttribute("value"));
                                }
                            }
                                

                           
                        }     
                    }
                    await PutTaskDelay(250);
                    webBrowser1.Document.GetElementsByTagName("input")["commit"].InvokeMember("click");

                    while (webBrowser1.Document?.GetElementById("cart")?.Document == null)
                        Application.DoEvents();

                    WaitForBrowserComplete();
                    await PutTaskDelay(500);
                }
                webBrowser1.Navigate("https://www.supremenewyork.com/checkout", null, null,
                    "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");

                WaitForBrowserComplete();
                int count = 0;
                while (webBrowser1.Document.GetElementById("order_billing_name") == null)

                {
                    if (count > 10000)
                        break;
                    count++;
                    WaitForBrowserComplete();
                }


                if (cbCountry.SelectedItem.ToString() == "JAPAN")
                {
                    webBrowser1.Document.GetElementById("credit_card_last_name").SetAttribute("value", txtName.Text);
                    webBrowser1.Document.GetElementById("credit_card_first_name").SetAttribute("value", txtlastName.Text);
                }
                else

                {
                    webBrowser1.Document.GetElementById("order_billing_name").SetAttribute("value", txtName.Text);
                    webBrowser1.Document.GetElementById("order_billing_country").SetAttribute("value", cbCountry.SelectedItem.ToString());
                }

              
               
                if (cbState.Visible == true)
                  webBrowser1.Document.GetElementById("order_billing_state").SetAttribute("value", cbState.SelectedItem.ToString());
                  
         
             
                webBrowser1.Document.GetElementById("order_email").SetAttribute("value", txtEmail.Text);
                webBrowser1.Document.GetElementById("order_tel").SetAttribute("value", txtTel.Text);
                webBrowser1.Document.GetElementById("order[billing_address]").SetAttribute("value", txtAddress1.Text);

                if (txtAddress2.Visible)
                    webBrowser1.Document.GetElementById("order[billing_address_2]").SetAttribute("value", txtAddress2.Text);

                if (txtAddress3.Visible)
                    webBrowser1.Document.GetElementById("order[billing_address_3]").SetAttribute("value", txtAddress3.Text);

            
                
                webBrowser1.Document.GetElementById("order_billing_city").SetAttribute("value", txtCity.Text);
                webBrowser1.Document.GetElementById("order_billing_zip").SetAttribute("value", txtPostcode.Text);
               
//#if DEBUG

//                webBrowser1.Document.GetElementById("credit_card_type").SetAttribute("value", "paypal");
//#else
                webBrowser1.Document.GetElementById("cnb").SetAttribute("value", txtNumber.Text);

                webBrowser1.Document.GetElementById("credit_card_month").SetAttribute("value", cbExpMonth.SelectedItem.ToString());
                webBrowser1.Document.GetElementById("credit_card_year").SetAttribute("value", cbExpYear.SelectedItem.ToString());
                webBrowser1.Document.GetElementById("credit_card_type").SetAttribute("value", cbType.SelectedItem.ToString());
//#endif

                while (solvedCaptchas.Count == 0)
                    Application.DoEvents();

                if (cbCountry.SelectedItem.ToString() == "CANADA")
                {
                    while (solvedCaptchas.Count < 1)
                        Application.DoEvents();

                }
                webBrowser1.Document.GetElementById("g-recaptcha-response").SetAttribute("innerHTML", solvedCaptchas[solvedCaptchas.Count - 1].Response);
                solvedCaptchas.RemoveAt(solvedCaptchas.Count - 1);
                webBrowser1.Document.GetElementById("vval").SetAttribute("value", txtCVV.Text);
                webBrowser1.Document.GetElementsByTagName("input")["order[terms]"].InvokeMember("click");
                webBrowser1.Document.GetElementById("order_terms").InvokeMember("Click");
              

                if (cbCountry.SelectedItem.ToString() == "CANADA")
                {
                    webBrowser1.Document.GetElementById("checkout_form").InvokeMember("submit");
                   await PutTaskDelay(_userPreference.SleepCheckout);
                    WaitForBrowserComplete();
                    while (webBrowser1.Document.GetElementById("state_label").InnerText != "province")
                        Application.DoEvents();
                    webBrowser1.Document.GetElementById("order_billing_state")
                        .SetAttribute("value", cbState.SelectedItem.ToString());
                    webBrowser1.Document.GetElementById("g-recaptcha-response").SetAttribute("innerHTML", solvedCaptchas[solvedCaptchas.Count - 1].Response);
                    solvedCaptchas.RemoveAt(solvedCaptchas.Count - 1);
                    webBrowser1.Document.GetElementById("checkout_form").InvokeMember("submit");
                }
                else
                {
                    await PutTaskDelay(_userPreference.SleepCheckout);
                    webBrowser1.Document.GetElementById("checkout_form").InvokeMember("submit");
                }
                succes = true;
            }
            catch (Exception ex)
            {

                succes = false;

            }

        }

        private bool LoadSettings()
        {
        
            if (!File.Exists(Application.StartupPath + @"\app.config"))
            {
                _userPreference = new UserPreference();
                _userPreference.ID = RandomString(5);
                return false;  
            }
            UserPreference obj = null;
            ICryptoTransform transform =
                 new DESCryptoServiceProvider().CreateDecryptor(key, key);

          
            using (Stream stream = new FileStream(Application.StartupPath + @"\app.config", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
           //    using (Stream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
             //  {
                  //  IFormatter formatter = new BinaryFormatter();
                   XmlSerializer serializer = new XmlSerializer(typeof(UserPreference));
                    obj = (UserPreference)serializer.Deserialize(stream);
            //   }
            }    
            ProductList = obj.ProductList;
            txtName.Text = obj.Name;
            txtEmail.Text = obj.Email;
            txtTel.Text = obj.Tel;
            txtAddress1.Text = obj.Address1;
            txtAddress2.Text = obj.Address2;
            txtAddress3.Text = obj.Address3;
            txtCity.Text = obj.City;
            cbCountry.SelectedIndex = obj.Country;
            cbState.SelectedIndex = obj.State;
            cbType.SelectedIndex = obj.Type;
            txtNumber.Text = StringCipher.Decrypt(obj.Number, Environment.MachineName);
            int x,y = 0;
            int.TryParse(StringCipher.Decrypt(obj.ExpYear, Environment.MachineName), out x );
            cbExpYear.SelectedIndex = x;
            int.TryParse(StringCipher.Decrypt(obj.ExpMonth, Environment.MachineName),out y);
            cbExpYear.SelectedIndex = y;
            txtCVV.Text = StringCipher.Decrypt(obj.Cvv, Environment.MachineName);
            txtPostcode.Text = obj.Zip;
            dateTimePicker1.Value = DateTime.Parse(obj.Alarm);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = ProductList;
            txtlastName.Text = obj.LastName;
            _userPreference = obj;

            return true;
        }
        private bool SaveSettings()
        {
          

     
            ICryptoTransform transform =
                new DESCryptoServiceProvider().CreateEncryptor(key,
                   key);

            
            _userPreference.ProductList = ProductList;
            _userPreference.Name = txtName.Text;
            _userPreference.Email = txtEmail.Text;
            _userPreference.LastName = txtlastName.Text;
            _userPreference.Tel = txtTel.Text;
            _userPreference.Address1 = txtAddress1.Text;
            _userPreference.Address2= txtAddress2.Text;
            _userPreference.Address3 = txtAddress3.Text;
            _userPreference.City = txtCity.Text;
            _userPreference.Zip = txtPostcode.Text;
            _userPreference.Country = cbCountry.SelectedIndex;
            _userPreference.State = cbState.SelectedIndex;
            _userPreference.Type = cbType.SelectedIndex;
            _userPreference.Number = StringCipher.Encrypt(txtNumber.Text, Environment.MachineName);
            _userPreference.ExpYear = StringCipher.Encrypt(cbExpYear.SelectedIndex.ToString(), Environment.MachineName);
            _userPreference.ExpMonth = StringCipher.Encrypt(cbExpMonth.SelectedIndex.ToString(), Environment.MachineName);
            _userPreference.Cvv = StringCipher.Encrypt(txtCVV.Text, Environment.MachineName);
            _userPreference.Alarm = dateTimePicker1.Value.ToString();
            using (var fs = new FileStream(Application.StartupPath + @"\app.config", FileMode.Create, FileAccess.Write))
            {
              XmlSerializer serializer = new XmlSerializer(typeof(UserPreference));
              serializer.Serialize(fs, _userPreference);
            }

            SchedulerMgr.CreateTask(_userPreference.ID, Application.ExecutablePath, "start", dateTimePicker1.Value.AddSeconds(-60));
            return true;
        }
        private readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }
        private bool findArticle(string category, string _productName, string _productColor)
        {
            bool find = false;
            webBrowser1.Navigate("http://www.supremenewyork.com/shop/all/" + category, null, null,
                    "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            WaitForBrowserComplete();
            HtmlElementCollection articleList = webBrowser1.Document.GetElementById("container").GetElementsByTagName("article");
            if (cbCountry.SelectedItem.ToString() == "JAPAN")
            {
                foreach (HtmlElement article in articleList)
                {
                    if (article.InnerHtml.Contains("sold_out_tag")) continue;
                    HtmlDocument html = GetHtmlDocument(article.InnerHtml);
                    string ProductUrl = "http://www.supremenewyork.com" +
                                        html.GetElementsByTagName("a")[0].GetAttribute("href").Replace("about:", "");

                    CookieCollection cookies = new CookieCollection();
                    //JAPAN FIX  
                    // do not find item directly
                    HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(ProductUrl);
                    getRequest.CookieContainer = new CookieContainer();
                    getRequest.CookieContainer.Add(cookies); //recover cookies First request
                    getRequest.Method = WebRequestMethods.Http.Get;
                    getRequest.UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                    getRequest.AllowWriteStreamBuffering = true;
                    getRequest.ProtocolVersion = HttpVersion.Version11;
                    getRequest.AllowAutoRedirect = true;
                    getRequest.ContentType = "application/x-www-form-urlencoded";
                    if (!string.IsNullOrWhiteSpace(_userPreference.Proxy) && string.IsNullOrEmpty(_userPreference.Proxy))
                    {
                        WebProxy myProxy = new WebProxy();
                        myProxy.Address = new Uri("http://" + _userPreference.Proxy);
                        getRequest.Proxy = myProxy;
                    }
                    string sourceCode;
                    HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                    using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                    {
                        sourceCode = sr.ReadToEnd();
                        if (sourceCode.ToLower().Contains(_productName.ToLower()) && sourceCode.ToLower().Contains("itemprop=\"model\">" + _productColor.ToLower()))
                        {
                            webBrowser1.Navigate(ProductUrl, null, null,
                    "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                            WaitForBrowserComplete();
                            find = true;
                            break;

                        }

                    }


                }
            }
            else
            {
                foreach (HtmlElement article in articleList)
                {
                    if (article.InnerHtml.Contains("sold_out_tag")) continue;
                    HtmlDocument html = GetHtmlDocument(article.InnerHtml);
                    string ProductName = html.GetElementsByTagName("h1")[0].InnerText;
                    string ProductColor = html.GetElementsByTagName("p")[0].InnerText;
                    string ProductUrl = "http://www.supremenewyork.com" + html.GetElementsByTagName("a")[0].GetAttribute("href").Replace("about:", "");
                    Console.WriteLine(ProductName + " | " + ProductColor);
                    if ((ProductName.ToLower().Contains(_productName.ToLower()) && ProductColor.ToLower() == _productColor.ToLower()) || (ProductName.ToLower() == _productName.ToLower() && ProductColor.ToLower() == _productColor.ToLower()))
                    {
                        webBrowser1.Navigate(ProductUrl, null, null,
                        "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                        WaitForBrowserComplete();
                        Application.DoEvents();
                        //await PutTaskDelay(500);
                        find = true;
                        break;
                    }
                }






            }

            return find;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            ProductForm pf = new ProductForm();
            if (pf.ShowDialog() == DialogResult.OK)
            {
                var a = new ProductInfo();
                a.Category = pf.PCategory;
                a.Name =pf.PName;
                a.Size =pf.PSize;
                a.Color = pf.PColor;
                a.SizeDefault = pf.P2SnSize;
                ProductList.Add(a);
            }
            dataGridView1.DataSource = null;
          
            dataGridView1.DataSource = ProductList;
          
        }

        private void button2_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewCell oneCell in dataGridView1.SelectedCells)
            {
                if (oneCell.Selected)
                 ProductList.RemoveAt(oneCell.RowIndex);
            }
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = ProductList;
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

            SettingsForm settingsForm =  new SettingsForm(_userPreference);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _userPreference.Proxy = settingsForm.Proxy;
                _userPreference.SleepCheckout = settingsForm.SleepCheckout;
                _userPreference.AutoBypassCaptcha = settingsForm.AutoBypassCaptcha;
            }
        }

        private void cbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCountry.SelectedItem.ToString() == "JAPAN")
            {
                LbState.Visible = true;
                cbState.Visible = true;
                LbState.Text = @"Province :";
                cbState.Items.Clear();
                cbState.Items.AddRange(new object[]
                {
                    " 北海道", " 青森県" , " 岩手県", " 宮城県", " 秋田県", " 山形県", " 福島県", " 茨城県", " 栃木県", " 群馬県"
                ," 埼玉県"," 千葉県"," 東京都"," 神奈川県"," 新潟県"," 富山県"," 石川県"," 福井県"," 山梨県"," 長野県"," 岐阜県"," 静岡県"," 愛知県"," 三重県"," 滋賀県"," 京都府"," 大阪府"," 兵庫県"," 奈良県"," 和歌山県"
                ," 鳥取県"," 島根県"," 岡山県"," 広島県"," 山口県"," 徳島県"," 香川県"," 愛媛県"," 高知県"," 福岡県"," 佐賀県"," 長崎県"," 熊本県"," 大分県"," 宮崎県"," 鹿児島県"," 沖縄県"
                });
                txtlastName.Visible = true;
                txtAddress2.Visible = false;
                txtAddress3.Visible = false;


            }

            else if (cbCountry.SelectedItem.ToString() == "CANADA")
            {
                LbState.Visible = true;
                cbState.Visible = true;
                txtlastName.Visible = false;
                LbState.Text = @"Province :";
                cbState.Items.Clear();
                cbState.Items.AddRange(new object[] { "AB","BC","MB","NB","NL","NT","NS","NU","ON","PE","QC","SK","YT"});
                txtAddress2.Visible = true;
                txtAddress3.Visible = false;
            }
            else if (cbCountry.SelectedItem.ToString() == "USA")
            {
                LbState.Visible = true;
                cbState.Visible = true;
                txtlastName.Visible = false;
                LbState.Text = @"State :";
                cbState.Items.Clear();
                cbState.Items.AddRange(new object[] {"AK","AS","AZ","AR","CA","CO","CT","DE","DC","FM","FL","GA","GU","HI","ID","IL","IN","IA","KS","KY","LA","ME","MH","MD","MA","MI","MN","MS","MO","MT","NE","NV","NH","NJ","NM","NY","NC","ND","MP","OH","OK","OR","PW","PA","PR","RI","SC","SD","TN","TX","UT","VT","VI","VA","WA","WV","WI","WY",});
                txtAddress2.Visible = true;
                txtAddress3.Visible = false;
            }
            else
            {
                LbState.Visible = false;
                cbState.Visible = false;
                txtlastName.Visible = false;
                txtAddress2.Visible = true;
                txtAddress3.Visible = true;
            }
        } 

        private bool succes = false;

        private void solveCaptcha()
        {

            while (true)
            {

                if ((DateTime.Now - StartingTime).TotalMinutes > 5)
                    Process.GetCurrentProcess().Kill();
                for (int i = 0; i < solvedCaptchas.Count; i++)
                {
                    if (!solvedCaptchas[i].isValid())
                        solvedCaptchas.RemoveAt(i);
                }
                bool canada = false;
                this.Invoke((MethodInvoker)delegate {
                    txtCaptcha.Text = solvedCaptchas.Count.ToString(); // runs on UI thread
                    if (cbCountry.SelectedItem.ToString() == "CANADA" && solvedCaptchas.Count < 1)
                        canada = true;
                });
                if (solvedCaptchas.Count == 0 && !SolverOpened)
                    GetSolvedCaptcha(_userPreference.AutoBypassCaptcha);
                if (canada)
                    GetSolvedCaptcha(_userPreference.AutoBypassCaptcha);
            }

        }
        private async void Form1_Shown(object sender, EventArgs e)
        {

            
         
            if (!_start)
                return ;
            //PurchaseHelper purchaseHelper = new PurchaseHelper(_userPreference);
            //purchaseHelper.Purchase();
            serverThread = new Thread(StartListener);
            serverThread.Start();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            StartingTime = DateTime.Now;
            captchaSolverThread = new Thread(solveCaptcha);
            captchaSolverThread.Start();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            Text = @"BOT-SUPREME.COM [AUTO-PILOT]";
            int count = 1000;
            RefreshIESettings(_userPreference.Proxy);
            while (count > 0 && !succes)
            {

                await Work();
                count--;
                System.Threading.Thread.Sleep(1200);
            }
            if (succes)
            {
                try
                {
                    captchaSolverThread.Abort();
                }
                catch (Exception exception)
                {
                }

                MessageBox.Show(
                  @"The purchase has been completed for " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + Environment.NewLine + @"Elapsed: " + stopwatch.Elapsed.Seconds, "", MessageBoxButtons.OK,
                  MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                  @"An error(521) has been occured during purchase", "", MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_userPreference.ID != null)
            {
             SchedulerMgr.DeleteTask(_userPreference.ID);

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.bot-supreme.com/update_file");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtNumber.UseSystemPasswordChar = !checkBox1.Checked;
            txtCVV.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshIESettings("");
            Process.GetCurrentProcess().Kill();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
          
       
           
        }

        public void GetSolvedCaptcha(bool auto)
        {
            SolverOpened = true;
            if (auto)
            {
                //6LeWwRkUAAAAAOBsau7KpuC9AV-6J8mhw4AjC3Xz
                var resolver = new _2Captcha("6LeWwRkUAAAAAOBsau7KpuC9AV-6J8mhw4AjC3Xz", "ccfe423c5c926492a75b35e60ed6670f");
                var response =  resolver.ResolveCaptcha();
                if (response != null)
                    solvedCaptchas.Add(new SolvedCaptcha(response));
                else
                {
                   GetSolvedCaptcha(false);
                }
                return;
            }
            else
            {

                if (!File.Exists(Application.StartupPath + "\\CaptchaSolver.exe")) return;
                Process.Start(Application.StartupPath + "\\CaptchaSolver.exe", listenPort.ToString()).WaitForExit();
                while (Process.GetProcessesByName("CaptchaSolver") != null)
                    Application.DoEvents();
                SolverOpened = false;
            }
        }
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {
            if (cbCountry.SelectedItem.ToString() != "FRANCE") return;
            txtAddress1.Text = RemoveSpecialCharacters(txtAddress1.Text);
            txtAddress1.BringToFront();
        }

        private void txtAddress2_TextChanged(object sender, EventArgs e)
        {
            if (cbCountry.SelectedItem.ToString() != "FRANCE") return;
            txtAddress2.Text = RemoveSpecialCharacters(txtAddress2.Text);
            txtAddress2.BringToFront();
        }

        private void txtAddress3_TextChanged(object sender, EventArgs e)
        {
            if (cbCountry.SelectedItem.ToString() != "FRANCE") return;
            txtAddress2.Text = RemoveSpecialCharacters(txtAddress3.Text);
            txtAddress2.BringToFront();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
      
                GetSolvedCaptcha(_userPreference.AutoBypassCaptcha);


        }

       
    }
}
