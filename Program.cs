using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

class Program
{
    static void Main()
    {
        string appTitle = @"
          _____      _            _         _  __            ______                       _            
         |  __ \    (_)          | |       | |/ /           |  ____|                     | |           
         | |__) | __ ___   ____ _| |_ ___  | ' / ___ _   _  | |__  __  ___ __   ___  _ __| |_ ___ _ __ 
         |  ___/ '__| \ \ / / _` | __/ _ \ |  < / _ \ | | | |  __| \ \/ / '_ \ / _ \| '__| __/ _ \ '__|
         | |   | |  | |\ V / (_| | ||  __/ | . \  __/ |_| | | |____ >  <| |_) | (_) | |  | ||  __/ |   
         |_|   |_|  |_| \_/ \__,_|\__\___| |_|\_\___|\__, | |______/_/\_\ .__/ \___/|_|   \__\___|_|   
                                                      __/ |             | |                            
                                                     |___/              |_|                            
        ";

        Console.WriteLine(appTitle);

        // Present user with store location options
        Console.WriteLine("Select Certificate Store location:");
        Console.WriteLine("1) Current User (default)");
        Console.WriteLine("2) Local Machine");
        Console.WriteLine("");
        Console.Write("Enter your choice (1 or 2): ");
        string choice = Console.ReadLine();

        StoreLocation location = StoreLocation.CurrentUser; // Default to CurrentUser
        if (choice == "2")
        {
            location = StoreLocation.LocalMachine;
        }
        else if (choice != "1")
        {
            Console.WriteLine("Invalid choice. Defaulting to Current User.");
        }

        // Define the certificate store to be accessed
        X509Store store = new X509Store(StoreName.My, location);

        Console.WriteLine("");

        try
        {
            store.Open(OpenFlags.ReadOnly);

            Console.Write("Enter the subject name (or part) of the certificate to search for: ");
            string subjectName = Console.ReadLine();
            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

            Console.WriteLine("");

            if (foundCertificates.Count > 0)
            {
                Console.WriteLine("Select a certificate:");
                Console.WriteLine("0) NOT LISTED");

                int index = 1;
                foreach (X509Certificate2 cert in foundCertificates)
                {
                    Console.WriteLine($"{index}) Subject: {cert.Subject}, Issuer: {cert.Issuer}, Created: {cert.NotBefore}, Key Exists: {cert.HasPrivateKey}");
                    index++;
                }

                Console.WriteLine("");

                Console.Write("Enter your choice: ");
                int selectedIndex = int.Parse(Console.ReadLine());

                Console.WriteLine("");

                if (selectedIndex == 0)
                {
                    Console.WriteLine("No certificate selected. Exiting.");
                    return;
                }
                else if (selectedIndex > 0 && selectedIndex <= foundCertificates.Count)
                {
                    X509Certificate2 selectedCertificate = foundCertificates[selectedIndex - 1];
                    HandleCertificate(selectedCertificate);
                }
                else
                {
                    Console.WriteLine("Invalid selection. Exiting.");
                }
            }
            else
            {
                Console.WriteLine("No certificates were found for the given subject name.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Check to ensure you have the correct privileges when running this application. Exiting.");
        }
        finally
        {
            store.Close();
        }
    }

    private static void HandleCertificate(X509Certificate2 certificate)
    {
        if (certificate.HasPrivateKey)
        {
            AsymmetricAlgorithm privateKey = null;

            // Try to get the private key as ECDsa
            if (certificate.GetECDsaPrivateKey() != null)
            {
                privateKey = certificate.GetECDsaPrivateKey();
                Console.WriteLine($"{privateKey.SignatureAlgorithm} private key found.\n");
            }
            // Otherwise, try to get the private key as RSA
            else if (certificate.GetRSAPrivateKey() != null)
            {
                privateKey = certificate.GetRSAPrivateKey();
                Console.WriteLine($"{privateKey.SignatureAlgorithm} private key found.\n");
            }

            // If a private key was found and extracted
            if (privateKey != null)
            {
                PromptAndSavePrivateKey(privateKey);
            }
            else
            {
                Console.WriteLine("No compatible private key found.");
            }
        }
        else
        {
            Console.WriteLine("No private key associated with the certificate.");
        }
    }

    private static void PromptAndSavePrivateKey(AsymmetricAlgorithm privateKey)
    {
        while (true)
        {
            Console.Write("Enter a file name to which the private key will be saved: ");
            string fileName = Console.ReadLine();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Default to MyDocuments
            string fullPath = Path.Combine(documentsPath, fileName); // Note, if the user provides a full path to a location outside MyDocuments it will save there, otherwise it defaults to MyDocuments

            // Path checking
            string fullPathDirectory = Path.GetDirectoryName(fullPath);

            if (Directory.Exists(fullPathDirectory)) {
                Console.WriteLine($"The private key will be saved to: {fullPath}");
                Console.WriteLine("The private key will not be encrypted. Protect it, and delete any copies after installation or use!");
                Console.Write("Agree to proceed? (Y/N): ");
                string confirmation = Console.ReadLine().Trim().ToLower();

                if (confirmation == "y" || confirmation == "yes")
                {
                    SavePrivateKeyToPem(privateKey, fullPath);
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter a new file name.");
                }
            } else
            {
                Console.WriteLine("The path that was entered is not valid. Please try again.");
            }
        }
    }
    private static void SavePrivateKeyToPem(AsymmetricAlgorithm privateKey, string outputPath)
    {
        byte[] pkcs8PrivateKey = privateKey.ExportPkcs8PrivateKey();
        StringBuilder pem = new StringBuilder();
        pem.AppendLine("-----BEGIN PRIVATE KEY-----");
        pem.AppendLine(Convert.ToBase64String(pkcs8PrivateKey, Base64FormattingOptions.InsertLineBreaks));
        pem.AppendLine("-----END PRIVATE KEY-----");

        try
        {
            File.WriteAllText(outputPath, pem.ToString());
            Console.WriteLine($"Private key saved as PEM encoded version of the PKCS#8 format to '{outputPath}'");
        }
        catch (Exception ex)
        { 
            Console.WriteLine(ex.Message); 
        }
        
    }
}
