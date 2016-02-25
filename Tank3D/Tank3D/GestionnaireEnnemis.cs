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
        int BorneMin { get; set; }
        int BorneMax { get; set; }
        int NbEnnemis { get; set; }
        List<AI> ListeEnnemis { get; set; }
        Random GénérateurAléatoire { get; set; }
        Joueur Cible { get; set; }
        Terrain TerrainJeu { get; set; }
        float ÉchelleAI { get; set; }
        float IntervalleMAJ { get; set; }

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
            BorneMin = (int)-TerrainJeu.Étendue.X / 2;
            BorneMax = (int)TerrainJeu.Étendue.X / 2;
            for (int i = 0; i < NbEnnemis; i++)
            {
                ListeEnnemis.Add(new AI(base.Game, "Veteran Tiger Forest", ÉchelleAI, Vector3.Zero, 
                                  new Vector3(GénérateurAléatoire.Next(BorneMin, BorneMax), 
                                  TerrainJeu.GetHauteur(TerrainJeu.ConvertionCoordonnées(new Vector3(GénérateurAléatoire.Next(BorneMin, BorneMax), 0, GénérateurAléatoire.Next(BorneMin, BorneMax)))), 
                                  GénérateurAléatoire.Next(BorneMin, BorneMax)), IntervalleMAJ, Cible));
                Game.Components.Add(ListeEnnemis[i]);
            }
        }
    }
}
