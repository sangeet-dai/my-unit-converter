using LearnHub.Helpers;
using LearnHub.Models;

namespace LearnHub.Data;

/// <summary>Seeds the local database with demo accounts, categories and learning resources.</summary>
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return; // database already seeded

        // ---------- Accounts ----------
        var admin = new ApplicationUser
        {
            FullName = "System Administrator",
            Email = "admin@learnhub.test",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            Role = Roles.Admin,
            Organisation = "LearnHub Team",
            Bio = "Administrator of the LearnHub digital learning platform."
        };

        var alice = new ApplicationUser
        {
            FullName = "Alice Tan",
            Email = "alice@learnhub.test",
            PasswordHash = PasswordHasher.Hash("Student123!"),
            Role = Roles.Member,
            Organisation = "Asia Pacific University",
            Bio = "Computing student exploring full-stack web development."
        };

        var bob = new ApplicationUser
        {
            FullName = "Bob Kumar",
            Email = "bob@learnhub.test",
            PasswordHash = PasswordHasher.Hash("Student123!"),
            Role = Roles.Member,
            Organisation = "Self-taught developer",
            Bio = "Career switcher learning data science and cloud computing."
        };

        var cara = new ApplicationUser
        {
            FullName = "Cara Lim",
            Email = "cara@learnhub.test",
            PasswordHash = PasswordHasher.Hash("Student123!"),
            Role = Roles.Member,
            Organisation = "Asia Pacific University",
            Bio = "Design enthusiast studying UI/UX and front-end frameworks."
        };

        admin.AvatarUrl = "/images/avatars/admin.png";
        alice.AvatarUrl = "/images/avatars/alice.png";
        bob.AvatarUrl = "/images/avatars/bob.png";
        cara.AvatarUrl = "/images/avatars/cara.png";

        db.Users.AddRange(admin, alice, bob, cara);

        // ---------- Categories ----------
        var catWeb = new Category { Name = "Web Development", IconClass = "ti ti-code", Description = "HTML, CSS, JavaScript and modern front-end frameworks.", LogoUrl = "/images/category/webdev.png" };
        var catProg = new Category { Name = "Programming", IconClass = "ti ti-terminal-2", Description = "Core programming concepts, algorithms and best practices.", LogoUrl = "/images/category/programming.png" };
        var catData = new Category { Name = "Data Science", IconClass = "ti ti-chart-bar", Description = "Data analysis, statistics and machine learning basics.", LogoUrl = "/images/category/datascience.png" };
        var catCloud = new Category { Name = "Cloud & DevOps", IconClass = "ti ti-cloud", Description = "Cloud platforms, containers and deployment pipelines.", LogoUrl = "/images/category/cloud.png" };
        var catDesign = new Category { Name = "Design & UX", IconClass = "ti ti-palette", Description = "Interface design, accessibility and user experience.", LogoUrl = "/images/category/designux.png" };
        var catSoft = new Category { Name = "Study Skills", IconClass = "ti ti-school", Description = "Learning strategies, productivity and academic skills.", LogoUrl = "/images/category/python.png" };
        db.Categories.AddRange(catWeb, catProg, catData, catCloud, catDesign, catSoft);

        // ---------- Learning resources ----------
        var resources = new List<LearningResource>
        {
            new() { Title = "Responsive Web Design with HTML5 & CSS3",
                Summary = "Build mobile-first, fully responsive web pages using semantic HTML5 and modern CSS layout techniques.",
                Description = "This interactive module walks you through the fundamentals of responsive design. You will learn how to structure pages with semantic HTML5 elements such as header, nav, main, section, article, aside and footer, then style them with CSS Flexbox and Grid. The module includes hands-on exercises where you re-build a real landing page so it adapts smoothly from wide desktop monitors down to small phone screens. Topics covered: the viewport meta tag, media queries, relative units, mobile-first breakpoint strategy, accessible navigation patterns and performance tips for production websites.",
                Objectives = "1. Structure web pages with semantic HTML5 elements.\n2. Apply CSS Grid and Flexbox for layout.\n3. Use media queries to create mobile-first responsive designs.\n4. Evaluate pages for accessibility and readability.",
                ResourceUrl = "https://developer.mozilla.org/en-US/docs/Learn/CSS/CSS_layout/Responsive_Design",
                Category = catWeb, ResourceType = ResourceTypeNames.Interactive, Level = LevelNames.Beginner, DurationMinutes = 150, IsFeatured = true, ThumbnailUrl = "/images/resource-responsive.svg", Publisher = admin },

            new() { Title = "JavaScript Fundamentals: Variables to Functions",
                Summary = "Master the core building blocks of JavaScript through runnable examples and mini-challenges.",
                Description = "A hands-on introduction to JavaScript for absolute beginners. Starting from variables and data types, you progress through operators, conditionals, loops, functions, arrays and objects. Every concept is demonstrated with runnable code snippets and followed by a short challenge so you learn by doing. The final section introduces the DOM and shows how JavaScript can react to user events on a web page.",
                Objectives = "1. Declare variables and choose correct data types.\n2. Write functions and use scope correctly.\n3. Manipulate arrays and objects.\n4. Select and update elements in the DOM.",
                ResourceUrl = "https://developer.mozilla.org/en-US/docs/Learn/JavaScript/First_steps", MediaSlug = "module-javascript",
                Category = catProg, ResourceType = ResourceTypeNames.Video, Level = LevelNames.Beginner, DurationMinutes = 180, IsFeatured = true, ThumbnailUrl = "/images/resource-javascript.svg", PictureUrl = "/images/resource/javascript-fundamentals.png", Publisher = admin },

            new() { Title = "ASP.NET Core MVC: Build Your First Web Application",
                Summary = "Step-by-step virtual lab creating a complete MVC web app with database connectivity.",
                Description = "In this virtual lab you scaffold an ASP.NET Core MVC project, create models, use Entity Framework Core with a local SQLite database, and implement the four CRUD operations (create, read, update, delete). The lab also covers cookie-based authentication, form validation with data annotations and publishing your application. Each step includes the exact commands and code you need, plus checkpoint quizzes to test your understanding before moving on.",
                Objectives = "1. Scaffold an ASP.NET Core MVC project.\n2. Connect the application to a SQLite database with EF Core.\n3. Implement insert, display, update and delete operations.\n4. Protect pages with role-based cookie authentication.",
                ResourceUrl = "https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/",
                Category = catWeb, ResourceType = ResourceTypeNames.VirtualLab, Level = LevelNames.Intermediate, DurationMinutes = 240, IsFeatured = true, ThumbnailUrl = "/images/resource-aspnet.svg", PictureUrl = "/images/resource/asp.net-core-mvc.png", Publisher = admin },

            new() { Title = "Python for Data Analysis",
                Summary = "Clean, explore and visualise real-world datasets using pandas and matplotlib.",
                Description = "Learn the essential data analysis workflow in Python. Using the pandas library you load CSV and Excel data, handle missing values, filter and aggregate datasets, and merge tables. You then create clear visualisations with matplotlib and seaborn to communicate your findings. All exercises use real public datasets so the skills transfer directly to coursework and industry projects.",
                Objectives = "1. Load and inspect tabular data with pandas.\n2. Clean messy datasets safely.\n3. Group, aggregate and reshape data.\n4. Produce insightful charts.",
                ResourceUrl = "https://pandas.pydata.org/docs/user_guide/10min.html",
                Category = catData, ResourceType = ResourceTypeNames.Article, Level = LevelNames.Intermediate, DurationMinutes = 200, ThumbnailUrl = "/images/resource-python.svg", PictureUrl = "/images/resource/python-for-data-analysis.png", Publisher = admin },

            new() { Title = "Introduction to Cloud Computing & DevOps",
                Summary = "Understand cloud service models, containers and CI/CD pipelines in one compact course.",
                Description = "A conceptual overview of cloud computing followed by practical exposure to modern DevOps tooling. You will learn the differences between IaaS, PaaS and SaaS, why containers changed deployment, and how continuous integration keeps code shippable. Short demonstrations show Docker containers in action and walk through a simple CI/CD pipeline that automatically tests and deploys a web application.",
                Objectives = "1. Compare IaaS, PaaS and SaaS deployment models.\n2. Explain the purpose of containers and images.\n3. Describe the stages of a CI/CD pipeline.\n4. Evaluate cloud solutions against project constraints.",
                ResourceUrl = "https://learn.microsoft.com/en-us/training/paths/az-900-describe-cloud-concepts/", MediaSlug = "module-cloud",
                Category = catCloud, ResourceType = ResourceTypeNames.Video, Level = LevelNames.Beginner, DurationMinutes = 120, ThumbnailUrl = "/images/resource-cloud.svg", PictureUrl = "/images/resource/introduction-to-cloud-computing-&-devops.png", Publisher = admin },

            new() { Title = "UI/UX Design Principles for Developers",
                Summary = "Apply visual hierarchy, colour theory and accessibility rules to your interfaces.",
                Description = "Great products are not only functional, they are usable and delightful. This module teaches the practical design principles every developer should know: visual hierarchy, alignment and spacing, colour theory, typography scales, and designing for accessibility. You will audit a real interface, identify usability problems, and redesign it using the techniques taught. Includes downloadable checklists you can reuse on your own projects.",
                Objectives = "1. Apply the principles of visual hierarchy and spacing.\n2. Choose accessible colour and type scales.\n3. Conduct a heuristic usability review.\n4. Produce low-fidelity wireframes.",
                ResourceUrl = "https://www.nngroup.com/articles/ten-usability-heuristics/",
                Category = catDesign, ResourceType = ResourceTypeNames.Article, Level = LevelNames.Beginner, DurationMinutes = 90, PictureUrl = "/images/resource/responsive-web-design-with-html5-&-css3.png", ThumbnailUrl = "/images/resource-ux.svg", Publisher = admin },

            new() { Title = "SQL Essentials: Querying Relational Databases",
                Summary = "Write confident SELECT, JOIN and aggregation queries with this interactive practice pack.",
                Description = "Structured Query Language is the universal language of data. This interactive module starts with simple SELECT statements and progressively introduces filtering, sorting, aggregate functions, GROUP BY, and multi-table joins. Every lesson is followed by practice queries you run directly in the browser against a sample company database, with instant feedback on your results.",
                Objectives = "1. Write SELECT queries with filters and sorting.\n2. Use aggregate functions and GROUP BY.\n3. Combine tables with INNER and LEFT JOIN.\n4. Design normalised table structures.",
                ResourceUrl = "https://sqlbolt.com/",
                Category = catData, ResourceType = ResourceTypeNames.Interactive, Level = LevelNames.Beginner, DurationMinutes = 130, IsFeatured = true, PictureUrl = "/images/resource/python-for-data-analysis.png", ThumbnailUrl = "/images/resource-sql.svg", Publisher = admin },

            new() { Title = "Algorithms & Data Structures Quiz Bank",
                Summary = "200+ self-assessment questions on lists, trees, sorting, searching and complexity.",
                Description = "Test your understanding of core computer science with this self-assessment quiz bank. Questions cover arrays, linked lists, stacks, queues, trees, graphs, sorting and searching algorithms, and big-O complexity analysis. Each question includes a full worked explanation so you learn from every attempt. Track your weak areas and revisit the linked theory notes before your exams.",
                Objectives = "1. Analyse algorithm time and space complexity.\n2. Choose appropriate data structures for a problem.\n3. Trace sorting and searching algorithms by hand.\n4. Identify knowledge gaps before examinations.",
                ResourceUrl = "https://www.bigocheatsheet.com/",
                Category = catProg, ResourceType = ResourceTypeNames.Quiz, Level = LevelNames.Intermediate, DurationMinutes = 60, PictureUrl = "/images/resource/responsive-web-design-with-html5-&-css3.png", ThumbnailUrl = "/images/resource-algorithms.svg", Publisher = admin },

            new() { Title = "Academic Writing & Study Skills Toolkit",
                Summary = "Note-taking systems, time management and citation basics for university success.",
                Description = "University study demands new skills. This toolkit introduces proven note-taking systems such as the Cornell method and mind mapping, time-management techniques like time-blocking and the Pomodoro technique, and the fundamentals of academic integrity including paraphrasing and citation. Printable templates and a one-week study planner are included so you can put the ideas into practice immediately.",
                Objectives = "1. Select an effective note-taking system.\n2. Plan weekly study time with time-blocking.\n3. Paraphrase and cite sources correctly.\n4. Recognise and avoid plagiarism.",
                ResourceUrl = "https://owl.purdue.edu/owl/research_and_citation/using_research/quoting_paraphrasing_and_summarizing/index.html",
                Category = catSoft, ResourceType = ResourceTypeNames.Article, Level = LevelNames.Beginner, DurationMinutes = 75, PictureUrl = "/images/resource/responsive-web-design-with-html5-&-css3.png", ThumbnailUrl = "/images/resource-study.svg", Publisher = admin },

            new() { Title = "Git & GitHub Collaboration Simulation",
                Summary = "Practise branching, pull requests and merge conflicts in a safe simulated repository.",
                Description = "Version control is essential for team software projects. This simulation drops you into a realistic team repository where you create feature branches, commit changes, open pull requests, request reviews and resolve merge conflicts — all without risking real code. By the end you will be comfortable with the everyday Git workflow used by professional development teams.",
                Objectives = "1. Create and switch branches confidently.\n2. Open and review pull requests.\n3. Resolve merge conflicts step by step.\n4. Write meaningful commit messages.",
                ResourceUrl = "https://learngitbranching.js.org/", MediaSlug = "module-git",
                Category = catProg, ResourceType = ResourceTypeNames.Simulation, Level = LevelNames.Intermediate, DurationMinutes = 100, ThumbnailUrl = "/images/resource-git.svg", PictureUrl = "/images/resource/git-and-github-collaboration-simulation.png", Publisher = admin }
        };
        db.Resources.AddRange(resources);

        // ---------- Enrolments & reviews ----------
        db.Enrollments.AddRange(
            new Enrollment { User = alice, Resource = resources[0], ProgressPercent = 100, Status = EnrollmentStatus.Completed, CompletedAt = DateTime.UtcNow.AddDays(-6) },
            new Enrollment { User = alice, Resource = resources[1], ProgressPercent = 65 },
            new Enrollment { User = alice, Resource = resources[6], ProgressPercent = 30 },
            new Enrollment { User = bob, Resource = resources[3], ProgressPercent = 80 },
            new Enrollment { User = bob, Resource = resources[4], ProgressPercent = 100, Status = EnrollmentStatus.Completed, CompletedAt = DateTime.UtcNow.AddDays(-12) },
            new Enrollment { User = cara, Resource = resources[5], ProgressPercent = 45 },
            new Enrollment { User = cara, Resource = resources[0], ProgressPercent = 90 }
        );

        db.Reviews.AddRange(
            new Review { User = alice, Resource = resources[0], Rating = 5, Comment = "The responsive exercises were excellent — I rebuilt my portfolio site mobile-first after finishing this module." },
            new Review { User = cara, Resource = resources[0], Rating = 4, Comment = "Very clear explanations. The media-query breakpoint section could use a few more examples, but overall a great module." },
            new Review { User = alice, Resource = resources[1], Rating = 5, Comment = "The mini-challenges after each video really helped the concepts stick. Perfect for beginners." },
            new Review { User = bob, Resource = resources[3], Rating = 4, Comment = "Real datasets made the pandas exercises feel relevant to actual work. Loved the visualisation section." },
            new Review { User = bob, Resource = resources[4], Rating = 4, Comment = "Concise and well structured. The CI/CD demonstration finally made pipelines make sense to me." },
            new Review { User = cara, Resource = resources[5], Rating = 5, Comment = "I apply the spacing and hierarchy checklists to every project now. Highly recommended for developers." }
        );

        db.SaveChanges();
    }
}
