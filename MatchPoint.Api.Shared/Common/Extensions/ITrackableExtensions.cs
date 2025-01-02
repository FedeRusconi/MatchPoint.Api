using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Shared.Common.Extensions
{
    public static class ITrackableExtensions
    {
        /// <summary>
        /// Set the fields defined by the <see cref="ITrackable"/> interface.
        /// Namely CreatedBy, CreatedOnUtc, ModifiedBy, ModifiedOnUtc
        /// </summary>
        /// <param name="trackableItem"> The item of item <see cref="ITrackable"/> to add the fields to.</param>
        /// <param name="userId"> The <see cref="Guid"/> of the user sending the request. </param>
        /// <param name="updating"> Pass true to update the "Modified" fields. Default is false. </param>
        /// <returns> The updated <see cref="ITrackable"/> item. </returns>
        public static ITrackable SetTrackingFields(this ITrackable trackableItem, Guid userId, bool updating = false)
        {
            if (updating)
            {
                trackableItem.ModifiedBy = userId;
                trackableItem.ModifiedOnUtc = DateTime.UtcNow;
            }
            else
            {
                trackableItem.CreatedBy = userId;
                trackableItem.CreatedOnUtc = DateTime.UtcNow;
            }

            return trackableItem;
        }
    }
}
