using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ionic.Zlib;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigSecureAP : UserControl
    {
        private AsymmetricCipherKeyPair keyPair;

        public ConfigSecureAP()
        {
            InitializeComponent();
        }


        private void but_privkey_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.DefaultExt = ".pem;.dat";
            openFileDialog1.Filter = "*.pem;*.dat|*.pem;*.dat";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var pem = File.ReadAllText(openFileDialog1.FileName);
                if (pem.Contains("PRIVATE_KEYV1"))
                {
                    pem = pem.Replace("PRIVATE_KEYV1:", "");
                    var keyap = Convert.FromBase64String(pem);
                    keyPair = SignedFW.GenerateKey(keyap);
                }
                else
                {
                    PemReader pr = new PemReader(new StringReader(pem));
                    var key = (Ed25519PrivateKeyParameters)pr.ReadObject();
                    keyPair = new AsymmetricCipherKeyPair(key.GeneratePublicKey(), key);
                }                
                txt_pubkey.Text = Convert.ToBase64String(((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded());
            }
        }

        private void but_bootloader_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.DefaultExt = ".bin";
            openFileDialog1.Filter = "*.bin|*.bin";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            txt_bl.Text = openFileDialog1.FileName;

            var ms = SignedFW.CreateSignedBL(keyPair, txt_bl.Text);

            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(txt_bl.Text),
                Path.GetFileNameWithoutExtension(txt_bl.Text) + "-signed.bin"), ms);

        }


        private void but_firmware_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.DefaultExt = ".apj";
            openFileDialog1.Filter = "*.apj|*.apj";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            var filename = openFileDialog1.FileName;

            txt_fwapj.Text = filename;

            var output = SignedFW.CreateSignedAPJ(keyPair, filename);

            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(filename),
                Path.GetFileNameWithoutExtension(filename) + "-signed.apj"), output);
        }


        private void but_generatekey_Click(object sender, EventArgs e)
        {
            keyPair = SignedFW.GenerateKey();

            TextWriter textWriter = new StringWriter();
            PemWriter pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(keyPair);
            pemWriter.Writer.Flush();
            var privatekey = pemWriter.Writer.ToString();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".pem";
            sfd.Filter = "*.pem|*.pem";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, privatekey);

                File.WriteAllText(sfd.FileName.Replace(".pem", "_private_key.dat"), "PRIVATE_KEYV1:" + Convert.ToBase64String(((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded()));
                File.WriteAllText(sfd.FileName.Replace(".pem", "_public_key.dat"), "PUBLIC_KEYV1:" + Convert.ToBase64String(((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded()));

                txt_pubkey.Text = Convert.ToBase64String(((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded());
                CustomMessageBox.Show("Protect your private key, if lost there is no method to get it back.");
            }
        }
    }
}
