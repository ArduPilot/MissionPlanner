using System;
using System.Security.Cryptography;
using System.Text;

namespace Rtsp
{

    // WWW-Authentication and Authorization Headers
    public class Authentication
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public enum Type {Basic, Digest};

        private String username = null;
        private String password = null;
        private String realm = null;
        private String nonce = null;
        private Type authentication_type = Type.Digest;
        private readonly MD5 md5 = System.Security.Cryptography.MD5.Create();


        private const char quote = '\"';

        // Constructor
        public Authentication(String username, String password, String realm, Type authentication_type) {
            this.username = username;
            this.password = password;
            this.realm = realm;
            this.authentication_type = authentication_type;

            this.nonce = new Random().Next(100000000,999999999).ToString(); // random 9 digit number            
        }

        public String GetHeader() {
            if (authentication_type == Type.Basic) {
                return "Basic realm=" + quote + realm + quote;
            }
            if (authentication_type == Type.Digest) {
                return "Digest realm=" + quote + realm + quote + ", nonce=" + quote + nonce + quote;
            }
            return null;
        }
        

		public bool IsValid(Rtsp.Messages.RtspMessage received_message) {
			
			string authorization = received_message.Headers["Authorization"];
            

			// Check Username and Password
            if (authentication_type == Type.Basic && authorization.StartsWith("Basic ")) {
                string base64_str = authorization.Substring(6); // remove 'Basic '
                byte[] data = Convert.FromBase64String(base64_str);
                string decoded = Encoding.UTF8.GetString(data);
                int split_position = decoded.IndexOf(':');
                string decoded_username = decoded.Substring(0, split_position);
                string decoded_password = decoded.Substring(split_position + 1);

                if ((decoded_username == username) && (decoded_password == password)) {
					_logger.Debug("Basic Authorization passed");
                    return true;
                } else {
					_logger.Debug("Basic Authorization failed");
                    return false;
                }
            }

            // Check Username, URI, Nonce and the MD5 hashed Response
            if (authentication_type == Type.Digest && authorization.StartsWith("Digest ")) {
                string value_str = authorization.Substring(7); // remove 'Digest '
                string[] values = value_str.Split(',');
                string auth_header_username = null;
				string auth_header_realm = null;
                string auth_header_nonce = null;
				string auth_header_uri = null;
                string auth_header_response = null;
				string message_method = null;
				string message_uri = null;
				try {
					message_method = received_message.Command.Split(' ')[0];
                    message_uri = received_message.Command.Split(' ')[1];
				} catch {}

                foreach (string value in values) {
                    string[] tuple = value.Trim().Split(new char[] {'='},2); // split on first '=' 
                    if (tuple.Length == 2 && tuple[0].Equals("username")) {
                        auth_header_username = tuple[1].Trim(new char[] {' ','\"'}); // trim space and quotes
                    }
					else if (tuple.Length == 2 && tuple[0].Equals("realm")) {
                        auth_header_realm = tuple[1].Trim(new char[] {' ','\"'}); // trim space and quotes
                    }
                    else if (tuple.Length == 2 && tuple[0].Equals("nonce")) {
						auth_header_nonce = tuple[1].Trim(new char[] {' ','\"'}); // trim space and quotes
                    }
                    else if (tuple.Length == 2 && tuple[0].Equals("uri")) {
						auth_header_uri = tuple[1].Trim(new char[] {' ','\"'}); // trim space and quotes
                    }
                    else if (tuple.Length == 2 && tuple[0].Equals("response")) {
						auth_header_response = tuple[1].Trim(new char[] {' ','\"'}); // trim space and quotes
                    }
                }

                // Create the MD5 Hash using all parameters passed in the Auth Header with the 
                // addition of the 'Password'
				String hashA1 = CalculateMD5Hash(md5, auth_header_username+":"+auth_header_realm+":"+this.password);
                String hashA2 = CalculateMD5Hash(md5, message_method + ":" + auth_header_uri);
                String expected_response = CalculateMD5Hash(md5, hashA1 + ":" + auth_header_nonce + ":" + hashA2);

                // Check if everything matches
				// ToDo - extract paths from the URIs (ignoring SETUP's trackID)
				if ((auth_header_username == this.username)
				    && (auth_header_realm == this.realm)
				    && (auth_header_nonce == this.nonce)
				    && (auth_header_response == expected_response)
				   ){
				    _logger.Debug("Digest Authorization passed");
                    return true;
                } else {
					_logger.Debug("Digest Authorization failed");
                    return false;
                }
            }
            return false;
        }



        // Generate Basic or Digest Authorization
        public string GenerateAuthorization(string username, string password,
                                            string auth_type, string realm, string nonce, string url, string command)  {

            if (username == null || username.Length == 0) return null;
            if (password == null || password.Length == 0) return null;
            if (realm == null || realm.Length == 0) return null;
            if (auth_type.Equals("Digest") && (nonce == null || nonce.Length == 0)) return null;

            if (auth_type.Equals("Basic")) {
                byte[] credentials = System.Text.Encoding.UTF8.GetBytes(username+":"+password);
                String credentials_base64 = Convert.ToBase64String(credentials);
                String basic_authorization = "Basic " + credentials_base64;
                return basic_authorization;
            }
            else if (auth_type.Equals("Digest")) {

                MD5 md5 = System.Security.Cryptography.MD5.Create();
                String hashA1 = CalculateMD5Hash(md5, username+":"+realm+":"+password);
                String hashA2 = CalculateMD5Hash(md5, command + ":" + url);
                String response = CalculateMD5Hash(md5, hashA1 + ":" + nonce + ":" + hashA2);

                const String quote = "\"";
                String digest_authorization = "Digest username=" + quote + username + quote +", "
                    + "realm=" + quote + realm + quote + ", "
                    + "nonce=" + quote + nonce + quote + ", "
                    + "uri=" + quote + url + quote + ", "
                    + "response=" + quote + response + quote;

                return digest_authorization;
            }
            else {
                return null;
            }

        }
        


        // MD5 (lower case)
        private string CalculateMD5Hash(MD5 md5_session, string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5_session.ComputeHash(inputBytes);

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                output.Append(hash[i].ToString("x2"));
            }

            return output.ToString();
        }
    }
}