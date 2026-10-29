namespace SmritivanWeb.Models
{
    public class GalleryImage
    {
        public int Pk { get; set; }

        public string Gallery_Image { get; set; } = "";

        public string Gallery_Path { get; set; } = "";

        public string? Title { get; set; }

        // ✅ IMPORTANT: used for filtering images by category
        public string? Album_Name { get; set; }
    }

    public class Album
    {
        // ❌ Removed Album_Id (not needed anymore)

        // ✅ Category name (Rebirth, Rebuild, etc.)
        public string Album_Name { get; set; } = "";

        // ✅ Thumbnail image (first image of that category)
        public string? Gallery_Image { get; set; }
    }
}