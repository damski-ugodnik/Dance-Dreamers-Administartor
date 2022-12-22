namespace Dance_Dreamers_Administartor.model
{
    public class Event
    {
        private int? id;
        private string name;
        private DateTime startDate;
        private DateTime? endDate;
        private string town;
        private string place;
        private string? additionalInfoUrl;

        public Event(int? id, string name, DateTime startDate, DateTime? endDate, string town, string place, string? additionalInfoUrl)
        {
            this.id = id;
            this.name = name.Trim();
            this.startDate = startDate;
            this.endDate = endDate;
            this.town = town.Trim();
            this.place = place.Trim();
            this.additionalInfoUrl = additionalInfoUrl;
        }

        public int? Id { get => id; }
        public string Name { get => name; }
        public DateTime StartDate { get => startDate; }
        public DateTime? EndDate { get => endDate; }
        public string Town { get => town; }
        public string Place { get => place; }
        public string? AdditionalInfoUrl { get => additionalInfoUrl;}
    }
}
