using SkillsHeroes.IssuesApi.Data;
using SkillsHeroes.IssuesApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillsHeroes.IssuesApi
{
    public static class IssuesSeeder
    {
        public static void AddSeedForKeyIfNeeded(this IssuesContext context, string key)
        {
            const string QUOTE_1 = "The path of the righteous man is beset on all sides by the iniquities of the selfish and the tyranny of evil men. Blessed is he who, in the name of charity and good will, shepherds the weak through the valley of darkness, for he is truly his brother's keeper and the finder of lost children. And I will strike down upon thee with great vengeance and furious anger those who would attempt to poison and destroy My brothers. And you will know My name is the Lord when I lay My vengeance upon thee.";
            const string QUOTE_2 = "You think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man.";
            const string QUOTE_3 = "Do you see any Teletubbies in here? Do you see a slender plastic tag clipped to my shirt with my name printed on it? Do you see a little Asian child with a blank expression on his face sitting outside on a mechanical helicopter that shakes when you put quarters in it? No? Well, that's what you see at a toy store. And you must think you're in a toy store, because you're here shopping for an infant named Jeb.";
            const string QUOTE_4 = "Normally, both your asses would be dead as fucking fried chicken, but you happen to pull this shit while I'm in a transitional period so I don't wanna kill you, I wanna help you. But I can't give you this case, it don't belong to me. Besides, I've already been through too much shit this morning over this case to hand it over to your dumb ass.";
            const string QUOTE_5 = "Now that we know who you are, I know who I am. I'm not a mistake! It all makes sense! In a comic, you know how you can tell who the arch-villain's going to be? He's the exact opposite of the hero. And most times they're friends, like you and me! I should've known way back when... You know why, David? Because of the kids. They called me Mr Glass.";

            if (!context.Applications.Any(a => a.ApiKey == key))
            {
                var application = context.Applications.Add(new Application() { ApiKey = key }).Entity;
                application.Issues = new HashSet<Issue>
                {
                    new Issue()
                    {
                        Title = "Issue 1",
                        Description = QUOTE_1,
                        Urgency = Urgency.High,
                        Created = new DateTime(2019, 3, 19, 10, 13, 12),
                        InProcess = new DateTime(2019, 3, 19, 11, 32, 21),
                        Completed = new DateTime(2019, 3, 19, 14, 43, 55),
                        Comments = new HashSet<Comment>()
                        {
                            new Comment(){ Text = "This is a good issue!" }
                        }
                    },
                    new Issue() { Title = "Issue 2", Description = QUOTE_2, Urgency = Urgency.Low, Created = new DateTime(2019, 3, 19, 12, 42, 31), InProcess = new DateTime(2019, 3, 19, 13, 32, 21) },
                    new Issue() { Title = "Issue 3", Description = QUOTE_3, Urgency = Urgency.Low, Created = new DateTime(2019, 3, 19, 13, 28, 1) },
                    new Issue() { Title = "Issue 4", Description = QUOTE_4, Urgency = Urgency.Low, Created = new DateTime(2019, 3, 19, 14, 49, 57) },
                    new Issue() { Title = "Issue 5", Description = QUOTE_5, Urgency = Urgency.Low, Created = new DateTime(2019, 3, 19, 15, 3, 43) }
                };
            }
        }
    }
}
