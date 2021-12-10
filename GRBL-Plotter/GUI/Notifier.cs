/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * 2020-12-18 Notifier by email or pushbullet 
 * 2021-07-15 code clean up / code quality
 * 2021-08-29 SendMessage async: https://docs.microsoft.com/de-de/dotnet/api/system.threading.tasks.task?view=netframework-4.0
*/

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GrblPlotter
{
    public static class Notifier
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public static void SendMessage(string message)
        { SendMessage(message,""); }
        public static void SendMessage(string message, string titleAddon)
        {
            bool mail = Properties.Settings.Default.notifierMailEnable;
            bool push = Properties.Settings.Default.notifierPushbulletEnable;
            if (!string.IsNullOrEmpty(message))
            {
                Logger.Info(culture, "Mail:{0} Push:{1}   Msg:{2}  Addon:{3}   Interval:{4}", mail, push, message.Replace("\r\n", " | "), titleAddon, Properties.Settings.Default.notifierMessageProgressInterval);
                if (mail)
                {
                    Task tm = Task.Factory.StartNew(() =>
                    {
                        SendMail(message, titleAddon);
                    });
                    tm.Wait();
                }

                if (push) 
                { 
                    Task tp = Task.Factory.StartNew(() =>
                    {
                        PushBullet(message, titleAddon);
                    });
                    tp.Wait();
                }
            }
        }

        public static string SendMail(string message, string titleAddon)
        {   // http://csharp.net-informations.com/communications/csharp-smtp-mail.htm
            try {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(Properties.Settings.Default.notifierMailClientAdr);

                mail.From = new MailAddress(Properties.Settings.Default.notifierMailSendFrom);
                mail.To.Add(Properties.Settings.Default.notifierMailSendTo);
                mail.Subject = Properties.Settings.Default.notifierMailSendSubject + " " + titleAddon;
                mail.Body = message;

                SmtpServer.EnableSsl = true;
                SmtpServer.Port = (int)Properties.Settings.Default.notifierMailClientPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.notifierMailClientUser, Properties.Settings.Default.notifierMailClientPass);

                SmtpServer.Send(mail);
                SmtpServer.Dispose();
                mail.Dispose();
                return "Email sent";
            }
            catch (Exception ex) {
                Logger.Error(ex, " sendMail() ");
                return "Error sending email:\r\n" + ex.ToString();
            }
        }

        public static string PushBullet(string message, string titleAddon = "")
        {
            Uri newUri = new Uri("https://api.pushbullet.com/v2/pushes");
            var httpWebRequest = WebRequest.Create(newUri);
            httpWebRequest.Headers.Add("Access-Token", Properties.Settings.Default.notifierPushbulletToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "";
                string channel = Properties.Settings.Default.notifierPushbulletChannel;
                if (channel.Length > 1)
                {
                    Channel info = new Channel
                    {
                        channel_tag = channel,
                        title = Properties.Settings.Default.notifierMailSendSubject + " " + titleAddon,
                        body = message
                    };
                    json = (new JavaScriptSerializer()).Serialize(info);
                }
                else
                {
                    Note info = new Note
                    {
                        title = Properties.Settings.Default.notifierMailSendSubject + " " + titleAddon,
                        body = message
                    };
                    json = (new JavaScriptSerializer()).Serialize(info);
                }
                Console.WriteLine(json);

                streamWriter.Write(json);
                streamWriter.Flush();
                //                streamWriter.Close();
            }

            try {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {   //var result = streamReader.ReadToEnd();
                    //return result;
                }
                return "PushBullet message sent";
            }
            catch (WebException ex) {
                return "Error sending PushBullet message:\r\n" + ex.ToString();
            }
            catch (Exception ex) {
                Logger.Error(ex, " pushBullet() ");
                return "Error sending PushBullet message:\r\n" + ex.ToString();
            }
        }
        internal class Note
        {
            public string type = "note";
            public string title = "Title here";
            public string body = "Insert body here";
        }
        internal class Channel
        {
            public string channel_tag = "channel_tag";
            public string type = "note";
            public string title = "Title here";
            public string body = "Insert body here";
        }

    }
}