using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   class TextureDeBase:IEquatable<TextureDeBase>
   {
      public string Nom { get; private set; }
      public Texture2D Image { get; private set; }
      ContentManager Content { get; set; }
      string Répertoire { get; set; }

      // Ce constructeur est appelé lorsque l'on construit un objet TextureDeBase
      // à partir d'une image déjà présente en mémoire.
      public TextureDeBase(string nom, Texture2D image)
      {
         Nom = nom;
         Content = null;
         Répertoire = "";
         Image = image;
      }

      // Ce constructeur est appelé lorsque l'on construit un objet TextureDeBase
      // à partir du nom d'une image qui sera éventuellement chargé en mémoire.
      public TextureDeBase(ContentManager content, string répertoire, string nom)
      {
         Nom = nom;
         Content = content;
         Répertoire = répertoire;
         Image = null;
      }

      public void Load()
      {
         if (Image == null)
         {
            string NomComplet = Répertoire + "/" + Nom;
            Image = Content.Load<Texture2D>(NomComplet);
         }
      }

      #region IEquatable<TextureDeBase> Membres

      public bool Equals(TextureDeBase other)
      {
         return Nom == other.Nom;
      }

      #endregion
   }
}
