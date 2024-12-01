using MatchPoint.Api.Shared.Interfaces;

namespace MatchPoint.Api.Shared.Extensions
{
    public static class ITrackableExtensions
    {
        /// <summary>
        /// Set the fields defined by the <see cref="ITrackable"/> interface.
        /// Namely CreatedBy, CreatedOnUtc, ModifiedBy, ModifiedOnUtc
        /// </summary>
        /// <param name="trackableItem"> The item of item <see cref="ITrackable"/> to add the fields to.</param>
        /// <param name="updating"> Pass true to update the "Modified" fields. Default is false. </param>
        /// <returns> The updated <see cref="ITrackable"/> item. </returns>
        public static ITrackable SetTrackingFields(this ITrackable trackableItem, bool updating = false)
        {
            if (updating)
            {
                // Add code to get current user
                trackableItem.ModifiedBy = Guid.NewGuid();
                trackableItem.ModifiedOnUtc = DateTime.UtcNow;
            }
            else
            {
                // Add code to get current user
                trackableItem.CreatedBy = Guid.NewGuid();
                trackableItem.CreatedOnUtc = DateTime.UtcNow;
            }

            return trackableItem;
        }
    }
}
