using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("settings.json");

            var config = builder.Build();

            using var store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindByThumbprint, config["CertificateThumbprint"], false);

            builder.AddAzureKeyVault(
                new Uri(config["VaultUri"]),
                new ClientCertificateCredential(
                    config["TenantId"],
                    config["ClientId"],
                cert.OfType<X509Certificate2>().Single()),
                new EnvironmentSecretManager("Development"));
            store.Close();

            config = builder.Build();

            Console.WriteLine(config["ConnectionString"]);
        }
    }
}
