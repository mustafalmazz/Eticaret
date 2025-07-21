using Eticaret.Core.Entities;

namespace Eticaret.WebUI.Models
{
    public class HomePageViemModel
    {
        public List<Slider>? Sliders { get; set; }
        public List<Product>? Products { get; set; }
        public List<News>? News { get; set; }
    }
}
