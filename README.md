# PrivateKeyExporter

PrivateKeyExporter is a utility designed for Windows administrators who need a straightforward way to extract private keys directly from the Windows certificate store. This tool is useful in scenarios where an application cannot use a certificate directly from the certificate store, and requires both the certificate and private keys to be installed separately. This eliminates the steps of exporting the certificate and private key to a PFX file first, then splitting them into individual components using another tool such as OpenSSL.

## Features

- **Easy Selection**: Choose certificates from either the Current User or Local Machine stores.
- **Flexible Search**: Find certificates by subject name.
- **Direct Export**: Export private keys directly without the need for intermediate PFX files.

## Getting Started

### Prerequisites

- Windows OS with .NET Framework.
- Administrator rights may be required depending on the certificate store location.

### Installation

1. Clone the repository or download the ZIP file.
2. Compile the solution using Visual Studio or a compatible .NET compiler.
3. Run the executable from the command line or through Visual Studio.

### Usage

1. Start the application. You will be prompted to select the certificate store location (Current User or Local Machine).
2. Enter the subject name of the certificate you are looking for. It will return results for any certificate subject that contains the search string.
3. Select the certificate from the list provided by the application.
4. Follow the prompts to export the private key, which will be saved as a PEM encoded file in the PKCS#8 format.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## Disclaimer

This tool is provided as-is, and the responsibility for secure handling and storage of private keys rests with the user. Always ensure private keys are handled securely to prevent unauthorized access.

## Contact

If you have any questions or feedback, please file an issue in the GitHub repository.
