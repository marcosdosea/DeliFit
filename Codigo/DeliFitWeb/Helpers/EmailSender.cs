using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace BibliotecaWeb.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();

                if (!MailboxAddress.TryParse(_configuration["Smtp:From"]?.Trim(), out var from))
                    throw new InvalidOperationException("O endereço de email remetente (Smtp:From) configurado no sistema é inválido.");

                if (!MailboxAddress.TryParse(email?.Trim(), out var to))
                    throw new InvalidOperationException($"O email do destinatário '{email}' é inválido.");

                message.From.Add(from);
                message.To.Add(to);

                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]),
                    SecureSocketOptions.StartTls
                );

                try
                {
                    await smtp.AuthenticateAsync(
                        _configuration["Smtp:Username"],
                        _configuration["Smtp:Password"]
                    );
                }
                catch (AuthenticationException authEx)
                {
                    throw new InvalidOperationException(
                        "Falha ao autenticar no servidor de email (SMTP). A senha de app configurada pode ter sido revogada ou expirado; gere uma nova senha de app do Gmail e atualize a configuração Smtp:Password.",
                        authEx);
                }

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);

                Console.WriteLine("Email enviado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}