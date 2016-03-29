using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class RessourceDeBase<T>:IEquatable<RessourceDeBase<T>>
    {
        public string Nom { get; private set; }
        public T Texture { get; private set; }
        ContentManager Content { get; set; }
        string Répertoire { get; set; }

        public RessourceDeBase(string nom, T texture)
        {
           Nom = nom;
           Content = null;
           Répertoire = "";
           Texture = texture;
        }

        public RessourceDeBase(ContentManager content, string répertoire, string nom)
        {
           Nom = nom;
           Content = content;
           Répertoire = répertoire;
           Texture = default(T);
        }

        public void Load()
        {
            if (Texture == null)
            {
                string NomComplet = Répertoire + "/" + Nom;
                Texture = Content.Load<T>(NomComplet);
            }
        }

        #region IEquatable<RessourceDeBase<T>> Membres

        public bool Equals(RessourceDeBase<T> other)
        {
            return Nom == other.Nom;
        }

        #endregion
    }
}
