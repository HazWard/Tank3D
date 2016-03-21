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
    public class Atelier : Microsoft.Xna.Framework.GameComponent
    {
        const string TITRE = "Tank 3D";
        const int NB_TUILES = 5;
        const int NB_ZONES = NB_TUILES + 1;
        public const float ÉCHELLE_OBJET = 0.05f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        protected const float HAUTEUR_CAM_DÉFAULT = 10f;
        protected const float DISTANCE_POURSUITE = 20f;
        MenuPrincipal MenuPrincipal { get; set; }
        MenuPause MenuPause { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        CaméraSubjective CaméraJoueur { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        NormalesManager GestionnaireDeNormales { get; set; }
        CalculateurFPS Calculateur { get; set; }
        InputManager GestionInput { get; set; }
        Terrain TerrainJeu { get; set; }
        Joueur Utilisateur { get; set; }
        Vector3 positionObjet { get; set; }
        Vector3 positionAI { get; set; }
        Vector3 positionTerrain { get; set; }
        Vector3 rotationObjet { get; set; }
        Vector3 positionCaméraSubjective { get; set; }
        Vector3 positionCaméra { get; set; }
        Vector3 cibleCaméra { get; set; }

        public Atelier(Game jeu)
            :base(jeu)
        {
        }

        public override void Initialize()
        {
            positionObjet = new Vector3(0, 10, 100);
            positionAI = new Vector3(-20, 10, 50);
            positionTerrain = new Vector3(0, 0, 0);
            rotationObjet = new Vector3(0, 0, 0); // MathHelper.PiOver2
            positionCaméraSubjective = new Vector3(0, 15, 15);
            positionCaméra = new Vector3(0, 100, 250);
            cibleCaméra = new Vector3(0, 0, -10);

            

            CaméraJoueur = new CaméraSubjective(Game, new Vector3(positionObjet.X, positionObjet.Y + HAUTEUR_CAM_DÉFAULT, positionObjet.Z + DISTANCE_POURSUITE),
                                               new Vector3(positionObjet.X, positionObjet.Y + 4, positionObjet.Z),
                                               Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(CaméraJoueur);
            Game.Services.AddService(typeof(Caméra), CaméraJoueur);

            TerrainJeu = new Terrain(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "PetiteCarte", "DétailsDésertSable", 3, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(TerrainJeu);
            Game.Services.AddService(typeof(Terrain), TerrainJeu);

            GestionInput = new InputManager(Game);
            Game.Components.Add(GestionInput);
            Calculateur = new CalculateurFPS(Game, INTERVALLE_CALCUL_FPS);
            Game.Components.Add(Calculateur);
            Game.Services.AddService(typeof(CalculateurFPS), Calculateur);
            Game.Components.Add(new Afficheur3D(Game));


            Game.Components.Add(new Sprite(Game, "crosshairBon", new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2), 0.2f));

            GestionEnnemis = new GestionnaireEnnemis(Game, Utilisateur, TerrainJeu, 0, ÉCHELLE_OBJET, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(GestionEnnemis);


            GestionnaireDeNormales = new NormalesManager(Game);
            Game.Components.Add(GestionnaireDeNormales);
            Game.Services.AddService(typeof(NormalesManager), GestionnaireDeNormales);

            Utilisateur = new Joueur(Game, "Veteran Tiger Body", ÉCHELLE_OBJET, rotationObjet, positionObjet, CaméraJoueur, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(Utilisateur);

            Game.Components.Add(new PlanTexturé(Game, 1f, Vector3.Zero, new Vector3(0, 6, -126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD));
            Game.Components.Add(new PlanTexturé(Game, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-126, 6, 0), new Vector2(256, 50), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD));
            Game.Components.Add(new PlanTexturé(Game, 1f, new Vector3(0, -(MathHelper.PiOver2), 0), new Vector3(126, 6, 0), new Vector2(256, 50), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD));
            Game.Components.Add(new PlanTexturé(Game, 1f, new Vector3(0, MathHelper.Pi, 0), new Vector3(0, 6, 126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD));
            Game.Components.Add(new PlanTexturé(Game, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 31, 0), new Vector2(256, 256), new Vector2(10, 10), "ciel", INTERVALLE_MAJ_STANDARD));

            MenuPause = new MenuPause(Game);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerClavier();
            base.Update(gameTime);
        }

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Escape))
            {
                if (!Game.Components.Contains(MenuPause))
                {
                    Game.Components.Add(MenuPause);
                }
                else
                {
                    MenuPause.Play();
                }
                //Exit();
            }
        }
        public void Écrire()
        {
            Console.WriteLine("bonjour");
        }
    }
}

