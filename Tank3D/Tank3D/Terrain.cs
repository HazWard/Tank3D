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
    public class Terrain : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        const int NB_SOMMETS_PAR_TUILE = 4;
        const float MAX_COULEUR = 255f;
        const float TEXTURE_ADJUST = 0.01f;
        public Vector3 Étendue { get; set; }
        string NomCarteTerrain { get; set; }
        string NomTextureTerrain { get; set; }
        int NbNiveauxTexture { get; set; }
        float PourcentageNiveauTexture { get; set; }
        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D CarteTerrain { get; set; }
        Texture2D TextureTerrain { get; set; }
        Vector3 Origine { get; set; }
        protected VertexPositionTexture[] Sommets { get; set; }
        protected Vector3[,] PtsSommets { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        int NbTuiles { get; set; }
        Vector3 Delta { get; set; }

        public int Extrêmes
        {
            get { return PtsSommets.GetLength(0) -1; }
        }

        // Gestion du HeightMap
        Color[] DataTexture { get; set; }

        public Terrain(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 étendue,
                           string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NomCarteTerrain = nomCarteTerrain;
            NbNiveauxTexture = nbNiveauxTexture;
            NomTextureTerrain = nomTextureTerrain;
            Étendue = étendue;
            Console.WriteLine(Étendue);
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            InitialiserDonnéesCarte();
            InitialiserDonnéesTexture();
            Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2); //pour centrer la primitive au point (0,0,0)
            PourcentageNiveauTexture = 1f / NbNiveauxTexture;
            AllouerTableaux();
            CréerTableauPoints();
            InitialiserSommets();
            base.Initialize();
        }

        void InitialiserDonnéesCarte()
        {
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            NbColonnes = CarteTerrain.Width;
            NbRangées = CarteTerrain.Height;
            Delta = new Vector3(Étendue.X / NbColonnes, Étendue.Y / MAX_COULEUR, Étendue.Z / NbRangées);
            NbTuiles = NbColonnes * NbRangées;
            NbSommets = NB_SOMMETS_PAR_TRIANGLE * NB_TRIANGLES_PAR_TUILE * NbTuiles;
            NbTriangles = NbSommets / NB_SOMMETS_PAR_TRIANGLE;
        }

        void InitialiserDonnéesTexture()
        {
            TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
            DataTexture = new Color[NbColonnes * NbRangées];
            CarteTerrain.GetData<Color>(DataTexture);
        }

        //
        // Allocation des deux tableaux
        //    1) celui contenant les points de sommet (les points uniques), 
        //    2) celui contenant les sommets servant à dessiner les triangles
        void AllouerTableaux()
        {
            PtsSommets = new Vector3[NbColonnes, NbRangées];
            Sommets = new VertexPositionTexture[NbSommets];
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
        }

        protected void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTerrain;
        }

        void CréerTableauPoints()
        {
            int compteur = 0;
            for (int j = 0; j < NbColonnes; ++j)
            {
                for (int i = 0; i < NbRangées; ++i)
                {
                    PtsSommets[i, j] = new Vector3(Origine.X + i * Delta.X, DataTexture[compteur].B * Delta.Y, Origine.Z - j * Delta.Z);
                    ++compteur;
                }
            }
        }

        int CalculerHauteurMoyenne(float chiffre1, float chiffre2, float chiffre3)
        {
            float moyenne = (chiffre1 + chiffre2 + chiffre3) / 3;
            float hauteurMoyenne = (moyenne / Étendue.Y / PourcentageNiveauTexture);
            return (int)hauteurMoyenne;
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            int niveau = 0;
            for (int j = 0; j < NbColonnes - 1; ++j)
            {
                for (int i = 0; i < NbRangées - 1; ++i)
                {
                    niveau = CalculerHauteurMoyenne(PtsSommets[i, j].Y, PtsSommets[i, j + 1].Y, PtsSommets[i + 1, j].Y);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], new Vector2(0, niveau * PourcentageNiveauTexture));
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], new Vector2(0, niveau * PourcentageNiveauTexture + PourcentageNiveauTexture - TEXTURE_ADJUST));
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], new Vector2(1, niveau * PourcentageNiveauTexture));

                    niveau = CalculerHauteurMoyenne(PtsSommets[i, j + 1].Y, PtsSommets[i + 1, j + 1].Y, PtsSommets[i + 1, j].Y);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], new Vector2(0, niveau * PourcentageNiveauTexture + PourcentageNiveauTexture - TEXTURE_ADJUST));
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], new Vector2(1, niveau * PourcentageNiveauTexture + PourcentageNiveauTexture - TEXTURE_ADJUST));
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], new Vector2(1, niveau * PourcentageNiveauTexture));
                }
            }
        }

        public float GetHauteur(Point coords)
        {
            // TODO: Utiliser des normales plus tard

            return PtsSommets[VérificationExtrêmes(coords.X), VérificationExtrêmes(coords.Y)].Y;
        }

        #region Calculs pour les normales

        public Vector2 GetNormale(Point coords)
        {
            Vector3 vecteurA = PtsSommets[coords.X, coords.Y + 1] - PtsSommets[coords.X, coords.Y];
            Vector3 vecteurB = PtsSommets[coords.X + 1, coords.Y] - PtsSommets[coords.X, coords.Y];

            Vector3 normale = Vector3.Normalize(Vector3.Cross(vecteurB, vecteurA));

            float angleX = (float)Math.Atan2(normale.X, normale.Y);
            float angleY = (float)Math.Atan2(normale.Z, normale.Y);

            Console.WriteLine("--------------------");
            Console.WriteLine("Angle en X: {0}", angleX);
            Console.WriteLine("Angle en Y: {0}", angleY);

            return new Vector2(-angleX, angleY);
        }

        /// <summary>
        /// Calcul de l'angle à l'aide de projection de vecteur
        /// </summary>
        /// <param name="axe">Axe de projection</param>
        /// <param name="vecteur">Vecteur à projeter</param>
        /// <returns>L'angle formé entre l'axe et le vecteur</returns>
        float AngleSurPlan(char axe, Vector3 vecteur)
        {
            float angle = 0;
            
            switch(axe)
            {
                case 'X':

                    break;

                case 'Y':

                    break;
            }
            return angle;
        }

        float AngleEntreDeuxVecteurs(Vector3 vecteurN, Vector3 vecteurV)
        {
            // Les deux vecteurs doivent être de norme 1
            float valeur = Vector3.Dot(vecteurN, vecteurV);
            return (float)Math.Acos(valeur);
        }

        #endregion

        #region Gestion des bornes
        // Vérification peu nécessaire
        int VérificationExtrêmes(int indice)
        {
            int maxPossible = PtsSommets.GetLength(0) - 1;       
            if(indice > maxPossible)
            {
                indice = maxPossible;
            }
            return indice;
        }

        public Point ConvertionCoordonnées(Vector3 coords)
        {
            int x = (int)Math.Abs((coords.X + Étendue.X / 2) / Delta.X);
            int y = (int)Math.Abs((coords.Z - Étendue.Z / 2) / Delta.Z);

            return new Point(x, y);
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
            }
            base.Draw(gameTime);
        }
    }
}