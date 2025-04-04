namespace MatchPoint.ClubService.Entities
{
    public class CourtFeatureLookupEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public object[] Choices { get; set; } = [];
    }

    /// Example of features from Grok:
    /// Lighting - Indicates whether the court has artificial lighting for night play (e.g., yes/no or specific type like None, LED, Floodlights).
    /// Indoor/Outdoor - Specifies if the court is indoors(protected from weather) or outdoors(exposed to elements).
    /// Seating Capacity - The number of spectators the court can accommodate, useful for tournament or community courts.
    /// Fencing - Whether the court is enclosed by fencing (e.g., fully enclosed, partially enclosed, or open).
    /// Condition - The current state of the court (e.g., excellent, good, fair, needs repair). - THIS SHOULD BE COVERED BY RATING
    /// Accessibility - Features like ramps or wheelchair-friendly surfaces for players with disabilities.
    /// Wall - Presence of a practice wall or backboard for solo training.
    /// Net Type - The quality or type of net (e.g., permanent, portable, standard, or premium).
    /// Court Markings - Whether the court has lines for singles, doubles, or multi-sport use (e.g., pickleball overlay).
    /// Screens - Availability of wind screens or shade structures for player comfort.
}
