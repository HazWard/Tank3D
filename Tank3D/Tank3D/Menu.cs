using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    class Menu:GameComponent
    {
        List<TexteCentré> ListeTextes { get; set; }
        ArrièrePlan ImageArrièrePlan { get; set; }
        BoutonDeCommande Jouer { get; set; }

        public Menu(Game jeu)
            :base(jeu)
        {
            
        }

        public override void Initialize()
        {
            ImageArrièrePlan = new ArrièrePlan(Game, "Background Tank");
            Jouer = new BoutonDeCommande(Game, "Jouer", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(100, 400), true, new FonctionÉvénemtielle(fonctioneéneéen));
            Game.Components.Add(ImageArrièrePlan);
            Game.Components.Add(Jouer);
            base.Initialize();
        }

        public void fonctioneéneéen() 
        {
            Game.Components.Remove(this);
            Game.Components.Remove(ImageArrièrePlan);
            Game.Components.Remove(Jouer);
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is IActivable)
                {
                    gc.Enabled = true;
                }
            }
        }
    }
}
