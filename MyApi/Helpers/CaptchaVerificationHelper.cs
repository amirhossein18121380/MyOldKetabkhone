using Common.Extension;
using Newtonsoft.Json;


namespace MyApi.Helpers
{
    public class CaptchaVerificationHelper
    {
        public async Task<bool> IsCaptchaValid(string token)
        {
            var result = false;

            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
            using var client = new HttpClient();

            try
            {

                var serverKey = "6LeV7rgbAAAAAHXdLRpoNi-KIgAk45TU_gVeZiu0";
                var response = await client.PostAsync($"{googleVerificationUrl}?secret={serverKey}&response={token}",
                    null);
                var jsonString = await response.Content.ReadAsStringAsync();
                //var captchaVerification = JsonConvert.DeserializeObject<CaptchaVerificationResponseViewModel>(jsonString);
                var captchaVerification = jsonString.ToModel<CaptchaVerificationResponseViewModel>();

                result = captchaVerification.Success;
                return result;
            }
            catch (Exception ex)
            {
                //await MongoLogging.ErrorLogAsync("CaptchaVerificationHelper|IsCaptchaValid", ex.Message, ex.StackTrace);
                return result;
            }
            finally
            {
                client.Dispose();
            }
        }
    }

    public class CaptchaVerificationResponseViewModel
    {
        [JsonProperty("success")] 
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public string ChallengeTs { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
