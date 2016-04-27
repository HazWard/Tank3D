using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{
    public class GestionnaireEnnemis : GameComponent
    {
        const int SAFE_SPOT_BORNES = 10;
        int BorneMin { get; set; }
        int BorneMax { get; set; }
        int NbEnnemis { get; set; }
        List<AI> ListeEnnemis { get; set; }
        Random GénérateurAléatoire { get; set; }
        Joueur Cible { get; set; }
        Terrain TerrainJeu { get; set; }
        float ÉchelleAI { get; set; }
        float IntervalleMAJ { get; set; }
        public bool DoitCréer { get; set; }

        public GestionnaireEnnemis(Game jeu, Joueur cible, Terrain terrainJeu, int nbEnnemis, float échelleAI, float intervalleMAJ)
            :base(jeu)
        {
            ListeEnnemis = new List<AI>();
            Cible = cible;
            TerrainJeu = terrainJeu;
            NbEnnemis = nbEnnemis;
            ÉchelleAI = échelleAI;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            base.Initialize();
            GénérateurAléatoire = new Random();
            DoitCréer = false;
            BorneMin = (int)-TerrainJeu.Étendue.X / 2 + 10;
            BorneMax = (int)TerrainJeu.Étendue.X / 2 - 10;
            for (int i = 0; i < NbEnnemis; i++)
            {
                ListeEnnemis.Add(new AI(base.Game, "Veteran Tiger Desert", ÉchelleAI, Vector3.Zero, 
                                  new Vector3(GénérateurAléatoire.Next(BorneMin, BorneMax),
                                  TerrainJeu.GetHauteur(TerrainJeu.ConvertionCoordonnées(new Vector3(GénérateurAléatoire.Next(BorneMin + SAFE_SPOT_BORNES, BorneMax - SAFE_SPOT_BORNES), 0, GénérateurAléatoire.Next(BorneMin, BorneMax)))),
                                  GénérateurAléatoire.Next(BorneMin + SAFE_SPOT_BORNES, BorneMax - SAFE_SPOT_BORNES)), IntervalleMAJ, Cible, i + 1, this));
                Game.Components.Add(ListeEnnemis[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (DoitCréer)
            {
                ListeEnnemis.Add(new AI(base.Game, "Veteran Tiger Desert", ÉchelleAI, Vector3.Zero,
                                    new Vector3(GénérateurAléatoire.Next(BorneMin, BorneMax),
                                    TerrainJeu.GetHauteur(TerrainJeu.ConvertionCoordonnées(new Vector3(GénérateurAléatoire.Next(BorneMin + SAFE_SPOT_BORNES, BorneMax - SAFE_SPOT_BORNES), 0, GénérateurAléatoire.Next(BorneMin, BorneMax)))),
                                    GénérateurAléatoire.Next(BorneMin + SAFE_SPOT_BORNES, BorneMax - SAFE_SPOT_BORNES)), IntervalleMAJ, Cible, NbEnnemis, this));
                Game.Components.Add(ListeEnnemis[ListeEnnemis.Count() - 1]);
                DoitCréer = false;
            }
            base.Update(gameTime);
        }

        public void EffacerEnnemis()
        {
            foreach (AI ennemi in ListeEnnemis)
            {
                Game.Components.Remove(ennemi.VieAI);
                Game.Components.Remove(ennemi);
            }
        }
    }
}
