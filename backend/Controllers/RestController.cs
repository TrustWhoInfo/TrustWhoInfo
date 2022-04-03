using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace backend
{
    [ApiController]
    [Route("api")]
    public class RestController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, Dictionary<string, Settings>> codes = new ConcurrentDictionary<string, Dictionary<string, Settings>>();
        private readonly ApplicationDbContext db;

        public RestController(ApplicationDbContext db) {
            this.db = db;
        }

        [HttpPost("generateCode")]
        public string GenerateCode(GenerateCodeRequest request) 
        {
            var code = Guid.NewGuid().ToString("N").Substring(0,8);
            codes[code] = request.Entities.ToDictionary(c=>c.Id, c=>c);
            return code;
        }

        [HttpPost("searchAuthor")]
        public List<Author> SearchAuthor(string pattern)
        {       
            const int limit = 100;
            if (string.IsNullOrWhiteSpace(pattern)) 
            {
                return db.Authors.OrderBy(c=>c.Id).Take(limit).ToList();
            }
            else 
            {
                return db.Authors
                    .Where(c=>c.Name.StartsWith(pattern, true, CultureInfo.CurrentCulture))
                    .Take(limit)
                    .ToList();
            }
        }

        [HttpPost("createAuthor")]
        public Author CreateAuthor(Author author) 
        {            
            db.Authors.Add(author);
            db.SaveChanges();        
            return author;
        }

        [HttpPost("postNews")]
        public Article PostNews(Article article) 
        {
            article.Timestamp = DateTime.UtcNow;
            db.Articles.Add(article);
            db.SaveChanges();        
            return article;
        }

        [HttpPost("media")]
        public List<Media> GetMedia(List<string> languages) 
        {
            List<Media> medias = new List<Media>{
            };
            return medias;
        }

        [HttpPost("authors")]
        public List<Author> GetAuthors(List<string> languages) 
        {
            if(true) return db.Authors.OrderBy(c=>c.Id).Take(100).ToList();
            List<Author> authors = new List<Author>{
                // new Author {
                //     Language = "en",
                //     Name = "Donald Trump",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/53/Donald_Trump_official_portrait_%28cropped%29.jpg/116px-Donald_Trump_official_portrait_%28cropped%29.jpg",
                //     Title = "Ex-president of USA",
                // },
                // new Author {
                //     Language = "en",
                //     Name = "Joe Biden",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9d/Joe_Biden_presidential_portrait_%28cropped%29.jpg/120px-Joe_Biden_presidential_portrait_%28cropped%29.jpg",
                //     Title = "President of USA",
                // },   
                // new Author {
                //     Language = "ch",
                //     Name = "Xi Jinping",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/32/Xi_Jinping_2019.jpg/120px-Xi_Jinping_2019.jpg",
                //     Title = "President of China",
                // },
                // new Author {
                //     Language = "ru",
                //     Name = "Vladimir Putin",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/Vladimir_Putin_%282020-02-20%29.jpg/113px-Vladimir_Putin_%282020-02-20%29.jpg",
                //     Title = "President of Russia",
                // },
                // new Author {
                //     Language = "de",
                //     Name = "Angela Merkel",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/bf/Angela_Merkel._Tallinn_Digital_Summit.jpg/116px-Angela_Merkel._Tallinn_Digital_Summit.jpg",
                //     Title = "Ex-chancellor of Germany",
                // },
                // new Author {
                //     Language = "sp",
                //     Name = "Pope Francis",
                //     Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/Pope_Francis_Korea_Haemi_Castle_19_%28cropped%29.jpg/120px-Pope_Francis_Korea_Haemi_Castle_19_%28cropped%29.jpg",
                //     Title = "Pope",
                // },
            };
            return authors;
        }

        private int? AuthorTrustLevel(Dictionary<string, Settings> entities, string authorId, List<string> subjects) {
            if (string.IsNullOrEmpty(authorId)) 
                return null;
            if (entities.TryGetValue(authorId, out var author)) {
                var subjectLevels = 0;
                var count = 0;
                foreach(var subject in subjects) {
                    if (author.Levels.TryGetValue(subject, out var level)) {
                        subjectLevels += level;
                        count++;
                    }
                }
                return count > 0 ? subjectLevels / count : author.Level;
            }
            return null;
        }

        // News structure: [statement, author] <= forward by author2 <= ... <= forward by authorN
        // News analytics: [analytic_id, news, trust level]
        // Input levels: media, news author, 
        private int TrustLevel(NewsInfo news, Dictionary<string, Settings> entities) {
            var levels = new List<int?> {
                AuthorTrustLevel(entities, news.Media, news.Subjects),
                AuthorTrustLevel(entities, news.Author, news.Subjects),
            }.Where(c=>c!=null).Select(c=>c.Value).ToList();
            if (levels.Count > 0) {
                var averageLevel = levels.Average();
                return (int) Math.Round(averageLevel);
            } else {
                return 0;
            }
        }

        [HttpPost("trustLevel")]
        public TrustLevelResponse TrustLevel(TrustLevelRequest request) {
            TrustLevelResponse response = new TrustLevelResponse();
            if (codes.TryGetValue(request.Code?.Trim() ?? "", out var entities)) {
                if (request.News != null) {
                    response.Ok = true;
                    foreach(var news in request.News) {
                        var level = TrustLevel(news, entities);
                        response.Levels.Add(level);
                    }
                } else {
                    response.Ok = false;
                    response.Error = "News is empty";
                }
            } else {
                response.Ok = false;
                response.Error = "Code not found";
            }
            return response;
        }

    }
}

public class GenerateCodeRequest {
    public List<Settings> Entities {get;set;}
}

public class Settings {
    public string Id {get;set;}
    public int Level {get;set;}
    public Dictionary<string, int> Levels {get;set;}
}

public class TrustLevelRequest {
    public string Code {get;set;}
    public List<NewsInfo> News {get;set;}
}

public class NewsInfo {
    // most probably news url
    public string Url {get;set;}
    
    // e.g. Facebook, CNN, BBC, WhatsApp, Telegram, Twitter etc
    public string Media {get;set;}     

    // Author Id
    public string? Author {get;set;} 

    // News subjects that could be used to specify trust level
    public List<string> Subjects {get;set;}
}

public class TrustLevelResponse {
    public bool Ok {get;set;}
    public string Error {get;set;}
    public List<int> Levels {get;set;} = new List<int>();
}

public class Author2 {
    public string Image {get;set;}
    public string Name {get;set;}
    public string Title {get;set;}
    public string Url {get;set;}
    public string Description {get;set;}

    public string Language {get;set;}
}

public class Media2 : Author2 {

}