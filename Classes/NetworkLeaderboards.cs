using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RocketJumper.Classes
{
    class NetworkLeaderboards
    {

        public static async Task PostScore(string username, long score, string date)
        {
            var json = JsonConvert.SerializeObject(new
            {
                username = username,
                score = score,
                date = date
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://localhost:5500", new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }

        public static async Task GetLeaderboards(string[] args)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:5500/scores");
            var scoresJson = await response.Content.ReadAsStringAsync();

            // write the scores to file
            System.IO.File.WriteAllText(MyGame.GlobalScoresFilePath, scoresJson);
        }
    }
}