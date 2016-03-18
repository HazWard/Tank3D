using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    class Menu:GameComponent
    {
        // Constantes
        const float POURCENTAGE_MARGE_HORIZONTALE = 0.05f;
        const float POURCENTAGE_MARGE_VERTICALE = 0.05f;
        ArrièrePlan ImageArrièrePlan { get; set; }
        BoutonDeCommande BtnJouer { get; set; }
        BoutonDeCommande BtnInstructions { get; set; }
        BoutonDeCommande BtnOptions { get; set; }
        BoutonDeCommande BtnQuitter { get; set; }
        BoutonDeCommande BtnRetourMenuPrincipal { get; set; }
        List<GameComponent> Boutons { get; set; }
        Instructions MenuInstructions { get; set; }
        
        public Menu(Game jeu)
            :base(jeu)
        {
            
        }

        public override void Initialize()
        {
            Boutons = new List<GameComponent>();
            ImageArrièrePlan = new ArrièrePlan(Game, "Background Tank");
            BtnJouer = new BoutonDeCommande(Game, "Jouer", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(100, 400), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnInstructions = new BoutonDeCommande(Game, "Instructions", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(230, 400), true, new FonctionÉvénemtielle(AfficherInstructions));
            BtnOptions = new BoutonDeCommande(Game, "Options", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(380, 400), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnQuitter = new BoutonDeCommande(Game, "Quitter", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(490, 400), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnRetourMenuPrincipal = new BoutonDeCommande(Game, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(750, 50), true, new FonctionÉvénemtielle(DémarrerJeu));
            Game.Components.Add(ImageArrièrePlan);
            Game.Components.Add(BtnJouer);
            Game.Components.Add(BtnInstructions);
            Game.Components.Add(BtnOptions);
            Game.Components.Add(BtnQuitter);
            base.Initialize();
        }

        void DémarrerJeu() 
        {
            EffacerMenu();
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is IActivable)
                {
                    gc.Enabled = true;
                }
            }
        }

        void EffacerMenu()
        {
            Game.Components.Remove(this);
            Game.Components.Remove(ImageArrièrePlan);
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is BoutonDeCommande)
                {
                    Boutons.Add(gc);
                }
            }
            foreach(BoutonDeCommande btn in Boutons)
            {
                Game.Components.Remove(btn);
            }
        }
        void AfficherInstructions()
        {
            int margeDroite = (int)(Game.Window.ClientBounds.Width * POURCENTAGE_MARGE_HORIZONTALE);
            int margeBas = (int)(Game.Window.ClientBounds.Height * POURCENTAGE_MARGE_HORIZONTALE);
            int margeGauche = 20;
            int margeHaut = 20;

            MenuInstructions = new Instructions(Game, "FondInstructions", new Rectangle(margeGauche, margeHaut, Game.Window.ClientBounds.Width - margeDroite,
                                                Game.Window.ClientBounds.Height - margeBas));
            Game.Components.Add(MenuInstructions);
            Game.Components.Add(BtnRetourMenuPrincipal);
        }
    }
}
