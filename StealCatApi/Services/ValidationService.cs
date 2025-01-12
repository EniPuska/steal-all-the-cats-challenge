using StealCat.Repositories.Interfaces;

namespace StealCatApi.Services
{
    public class ValidationService
    {
        // Validate that the page and pageSize are valid
        public static void ValidatePaging(int page, int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentException("Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                throw new ArgumentException("Page size must be between 1 and 100.");
            }
        }

        // Validate that a tag is not null or empty
        public static void ValidateTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Tag cannot be null or empty.");
            }
        }

        // Validate that a cat ID exists in the database
        public static async Task ValidateCatExists(ICatRepository catRepository, int catId)
        {
            var cat = await catRepository.GetCatById(catId);
            if (cat == null)
            {
                throw new ArgumentException($"Cat with ID {catId} does not exist.");
            }
        }

        // Validate the image URL
        public static void ValidateImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid image URL.");
            }
        }
    }
}
