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
        Game Jeu { get; set; }
        List<GameComponent> ListeGameComponentsMenu { get; set; }
        List<GameComponent> ListeGameComponentsAtelier { get; set; }
        List<GameComponent> ListeGameComponentsTanksDétruits { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        ArrièrePlan ImageArrièrePlan { get; set; }
        BoutonDeCommande ReprendreJeu { get; set; }
        BoutonDeCommande BtnRetourMenuPrincipal { get; set; }

        public MenuPause(Game jeu, List<GameComponent> listeGameComponentsMenu, List<GameComponent> listeGameComponentsAtelier, GestionnaireEnnemis gestionEnnemis)
            : base(jeu)
        {
            Jeu = jeu;
            ListeGameComponentsMenu = listeGameComponentsMenu;
            ListeGameComponentsAtelier = listeGameComponentsAtelier;
            GestionEnnemis = gestionEnnemis;
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
            ListeGameComponentsTanksDétruits = new List<GameComponent>();
            ImageArrièrePlan = new ArrièrePlan(Game, "Background Transparent 60");
            ReprendreJeu = new BoutonDeCommande(Game, "Reprendre le jeu", "Arial20", "BoutonNormal", "BoutonEnfoncé", new Vector2(2 * Game.Window.ClientBounds.Width / 8f, 6 * Game.Window.ClientBounds.Height / 10f), true, new FonctionÉvénemtielle(Play));
            BtnRetourMenuPrincipal = new BoutonDeCommande(Game, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(15f * (Game.Window.ClientBounds.Width / 16f), Game.Window.ClientBounds.Height / 10f), true, new FonctionÉvénemtielle(RetournerMenuPrincipal));
            Game.Components.Add(ImageArrièrePlan);
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
            Game.Components.Remove(ReprendreJeu);
            Game.Components.Remove(BtnRetourMenuPrincipal);
        }

        public void RetournerMenuPrincipal()
        {
            EffacerMenu();
            Game.Services.RemoveService(typeof(Caméra));

            foreach (GameComponent gc in ListeGameComponentsAtelier)
            {
                Game.Components.Remove(gc);
            }
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is TankDétruit)
                {
                    ListeGameComponentsTanksDétruits.Add(gc);
                }
            }
            foreach (GameComponent gc in ListeGameComponentsTanksDétruits)
            {
                Game.Components.Remove(gc);
            }
            GestionEnnemis.EffacerEnnemis();
            
            MenuPrincipal.ModifyComponents(true, ListeGameComponentsMenu);
        }

        void ArrêterJeu()
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
