using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Timers;

namespace CheckIpService
{
    public partial class CheckIpService : ServiceBase
    {
        private readonly Timer _timer = new Timer();

        public CheckIpService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.AutoReset = true;
            _timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["ServiceInterval"]);
            _timer.Elapsed += OnElapsedTime;
            _timer.Start();
        }

        private string _lastIp;
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                var client = new WebClient();
                var response = client.DownloadString("http://checkip.dyndns.org");
                const string startToken = "Current IP Address: ";
                var startIndex = response.IndexOf(startToken);
                startIndex += startToken.Length;
                var endIndex = response.IndexOf("</body>");
                var ip = response.Substring(startIndex, endIndex - startIndex);

                if (String.IsNullOrEmpty(_lastIp) || _lastIp != ip)
                {
                    _lastIp = ip;
                    SendMail(ip);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message);
            }
        }

        private void SendMail(string ip)
        {
            var appSettings = ConfigurationManager.AppSettings;

            var email = Convert.ToString(appSettings["EmailAddress"]);
            var displayName = Convert.ToString(appSettings["DisplayName"]);

            var fromAddress = new MailAddress(email, displayName);
            var toAddress = new MailAddress(email, displayName);
            var fromPassword = Convert.ToString(appSettings["EmailPassword"]);
            var subject = "Current Home IP Address: " + ip;
            var body = subject;
            var smtp = new SmtpClient
                            {
                                Host = Convert.ToString(appSettings["EmailHost"]),
                                Port = Convert.ToInt32(appSettings["EmailPort"]),
                                EnableSsl = Convert.ToBoolean(appSettings["EnableSsl"]),
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                            };
            using (var message = new MailMessage(fromAddress, toAddress)
                                        {
                                            Subject = subject,
                                            Body = body
                                        })
            {
                smtp.Send(message);
            }
        }

        protected override void OnStop()
        {
            _timer.Stop();
        }
    }
}
