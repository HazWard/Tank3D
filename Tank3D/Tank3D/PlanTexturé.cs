using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   public class PlanTexturé : Plan
   {
      string NomTexture { get; set; }
      RessourcesManager<Texture2D> TextureManager { get; set; }
      Texture2D Texture { get; set; }
      protected VertexPositionTexture[] Sommets { get; set; }
      Vector2[,] PtsTexture { get; set; }
      BlendState GestionAlpha { get; set; }
      public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
      {
         NomTexture = nomTexture;
      }
      protected override void LoadContent()
      {
         TextureManager = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         Texture = TextureManager.Find(NomTexture);
         base.LoadContent();
      }

      private void CréerTableauPointsTexture()
      {
         PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
         float DeltaX = 1f/NbColonnes;
         float DeltaY = 1f / NbRangées;
         for (int i = 0; i <= NbColonnes; ++i)
         {
            for (int j = 0; j <= NbRangées; ++j)
            {
               PtsTexture[i, j] = new Vector2(i * DeltaX, (NbRangées - j) * DeltaY);
            }
         }
      }

      protected override void InitialiserSommets()
      {
         CréerTableauPointsTexture();
         int positionDansLeTableau = 0;
         for (int i = 0; i < NbRangées; ++i)
         {
            for (int j = 0; j <= NbColonnes; ++j)
            {
               Sommets[positionDansLeTableau++] = new VertexPositionTexture(PtsSommets[j, i], PtsTexture[j, i]);
               Sommets[positionDansLeTableau++] = new VertexPositionTexture(PtsSommets[j, i + 1], PtsTexture[j, i+1]);
            }
         }
      }
      

      protected override void DessinerTriangleStrip(int noStrip)
      {
         GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, noStrip * (NbTrianglesParStrip + 2), NbTrianglesParStrip);
      }

      protected override void InitialiserParamètresEffetDeBase()
      {
         EffetDeBase.TextureEnabled = true;
         EffetDeBase.Texture = Texture;
         GestionAlpha = BlendState.AlphaBlend;
         
      }

      protected override void CréerTableauSommets()
      {
         Sommets = new VertexPositionTexture[(NbTrianglesParStrip + 2) * NbRangées];
      }
      public override void Draw(GameTime gameTime)
      {
         BlendState oldBlendState = GraphicsDevice.BlendState;
         GraphicsDevice.BlendState = GestionAlpha;
         base.Draw(gameTime);
         GraphicsDevice.BlendState = oldBlendState;
      }
   }
}
