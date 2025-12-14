using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharingNotes.Models;

namespace SharingNotes.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReadMore(int id)
        {
            Console.WriteLine(id);
            if (id == 1)
            {
                ViewBag.Title = "HTML";
                ViewBag.Description = "HTML (HyperText Markup Language) is the foundational language used to structure content on the web. " +
"It provides the basic building blocks of every webpage, defining elements such as headings, paragraphs, " +
"images, links, tables, lists, and forms. HTML works by using a system of tags and attributes that tell " +
"the browser how to interpret and display content. Over time, HTML has evolved to support semantic elements " +
"like <header>, <footer>, <article>, and <section>, which improve accessibility and SEO by giving meaning " +
"to different parts of a webpage. Modern HTML5 also introduces powerful features such as audio, video, " +
"canvas drawing, and form validation without relying on external plugins. Whether you're building a simple " +
"static page or a complex web application, HTML serves as the backbone that organizes and presents your content.";
            }
            else
            {
                ViewBag.Title = "CSS";
                ViewBag.Description = "CSS (Cascading Style Sheets) is the styling language used to control the appearance and layout of web pages. " +
"It allows developers to define colors, fonts, spacing, borders, backgrounds, animations, and responsive designs " +
"that adapt to different screen sizes. CSS separates presentation from structure, enabling cleaner and more " +
"maintainable code. With modern CSS features like Flexbox, Grid, transitions, and keyframe animations, developers " +
"can create visually appealing and highly interactive user interfaces. CSS also supports media queries, which make " +
"websites responsive across devices such as mobiles, tablets, and desktops. From simple color changes to complex " +
"layouts and animations, CSS brings creativity and visual polish to the web.";


            }

            return View();
        }
    }
}
