public class EventDeck
{
    public enum CardTypes {CurrentSectorSound, AnySectorSound, NoSound}
    
    ///// To future advanced game /////
    // CurrentSectorSound 10 (4 items)
    // AnySectorSound 10 (4 items)
    // NoSound 5

    ExtendedList<CardTypes> deck;

    public EventDeck(){
        deck = CreateDeck();
    }

    public ExtendedList<CardTypes> CreateDeck(){
        ExtendedList<CardTypes> deck = new ExtendedList<CardTypes>();

        deck.AddRedundant(CardTypes.CurrentSectorSound, 10);
        deck.AddRedundant(CardTypes.AnySectorSound, 10);
        deck.AddRedundant(CardTypes.NoSound, 5);

        deck.Shuffle();

        return deck;
    }

    public CardTypes DrawCard(){
        if(deck.Count == 0) deck = CreateDeck();

        return deck.PopValue();
    }
}
