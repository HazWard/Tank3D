using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    class MenuPause : GameComponent
    {
        List<GameComponent> Boutons { get; set; }
        Arri�rePlan ImageArri�rePlan { get; set; }
        BoutonDeCommande ReprendreJeu { get; set; }
        BoutonDeCommande BtnRetourMenuPrincipal { get; set; }

        public MenuPause(Game jeu)
            : base(jeu)
        {
        }

        public override void Initialize()
        {
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is IActivable)
                {
                    gc.Enabled = false;
                }
            }
            Boutons = new List<GameComponent>();
            ImageArri�rePlan = new Arri�rePlan(Game, "Background Transparent 60");
            ReprendreJeu = new BoutonDeCommande(Game, "Reprendre le jeu", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(100, 400), true, new Fonction�v�nemtielle(Play));
            BtnRetourMenuPrincipal = new BoutonDeCommande(Game, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(750, 50), true, new Fonction�v�nemtielle(RetourMenuPrincipal));
            Game.Components.Add(ImageArri�rePlan);
            Game.Components.Add(ReprendreJeu);
            Game.Components.Add(BtnRetourMenuPrincipal);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public void Play()
        {
            Game.Components.Remove(this);
            Game.Components.Remove(ImageArri�rePlan);
            Game.Components.Remove(ReprendreJeu);
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
            Game.Components.Remove(ImageArri�rePlan);
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is BoutonDeCommande)
                {
                    Boutons.Add(gc);
                }
            }
            foreach (BoutonDeCommande btn in Boutons)
            {
                Game.Components.Remove(btn);
            }
        }

        void RetourMenuPrincipal()
        {
            Game.Components.Remove(ImageArri�rePlan);
            Game.Components.Remove(ReprendreJeu);
            Game.Components.Remove(BtnRetourMenuPrincipal);
        }

        void Arr�terJeu()
        {
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is IActivable)
                {
                    gc.Enabled = false;
                }
            }
        }

        void RecommencerJeu()
        {
            Game.Components.Clear();
        }
    }
}
