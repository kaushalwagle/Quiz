using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuizApp.Models;

namespace QuizApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //Seed if necessary
            _ = SeedQuizesAsync(app.ApplicationServices);
        }

        private async Task SeedQuizesAsync(IServiceProvider serviceProvider) {
            
            //Referenced  https://github.com/kaushalwagle/4yearplanner/blob/master/Startup.cs
            //Seed if the quiz table is empty
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (!context.Quizes.Any()) {

                    context.Quizes.AddRange(await GetQuestionsFromOpenDatabase());
                }
                context.SaveChanges();
            }

        }

        public async Task<IList<Quiz>> GetQuestionsFromOpenDatabase() {
            IList<Quiz> quizes = new List<Quiz>();
            try {
                string openTriviaResponseJSON = await GetAsync(@"https://opentdb.com/api.php?amount=50&category=9&difficulty=easy&type=multiple");

                //Referenced from https://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm

                JObject openTriviaResponse = JObject.Parse(openTriviaResponseJSON);

                // get JSON result objects into a list
                IList<JToken> results = openTriviaResponse["results"].Children().ToList();

                // serialize JSON results into .NET objects
                foreach (JToken result in results) {
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    quizes.Add(new Quiz {
                        Question = WebUtility.HtmlDecode(result["question"].ToString()),
                        Answer = WebUtility.HtmlDecode(result["correct_answer"].ToString()),
                        Incorrect1 = WebUtility.HtmlDecode(result["incorrect_answers"][0].ToString()),
                        Incorrect2 = WebUtility.HtmlDecode(result["incorrect_answers"][1].ToString()),
                        Incorrect3 = WebUtility.HtmlDecode(result["incorrect_answers"][2].ToString())
                    });
                }
            }
            catch (Exception e) {

                Console.WriteLine(e.StackTrace);

                var quizesJSON = System.IO.File.ReadAllText(@"json/Quizes.json");
                quizes = JsonConvert.DeserializeObject<List<Quiz>>(quizesJSON);
            }
            return quizes;
        }


        //referenced from: https://stackoverflow.com/questions/27108264/c-sharp-how-to-properly-make-a-http-web-get-request
        private async Task<string> GetAsync(string uri) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
