using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SkillsHeroes.IssuesApi.Data;

namespace SkillsHeroes.IssuesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDbContext<IssuesContext>((context, provider) =>
                {
                    context.AddSeedForKeyIfNeeded("DEV_TEST_1");
                    context.AddSeedForKeyIfNeeded("tWEkgV34dJbSUuwQBxVCJKmf");
                    context.AddSeedForKeyIfNeeded("wXhvZjRQabuS3sdZjABK2RNU");
                    context.AddSeedForKeyIfNeeded("HZXvHTb2gqq3dYGCY2EUv49N");
                    context.AddSeedForKeyIfNeeded("sGX3pyFQ3SrTq4qWYq2A5vvx");
                    context.AddSeedForKeyIfNeeded("bwqqqxujr9dPsqezr4ZvzHey");
                    context.AddSeedForKeyIfNeeded("5h33Km3pVvfq3tbhPVs8cKx7");
                    context.AddSeedForKeyIfNeeded("SZEXGLsRCjj3BvsRCKXCnfan");
                    context.AddSeedForKeyIfNeeded("wAxEeVy6mwbL7WTYTy24FHcP");

                    context.SaveChanges();
                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(k =>
                {
                    k.ListenAnyIP(5000);
                })
                .UseStartup<Startup>();
    }
}
