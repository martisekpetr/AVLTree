using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVLstromy
{
    /*---------------------------------------------------
     * Zapoctovy program - implementace AVL stromu
     * Petr Martisek, kruh 39
     * LS 2011/12, predmet Algoritmy a datove struktury
     * cviceni vedene Vladanem Majerechem
    ---------------------------------------------------*/

    public class AVLstrom
    {
        private int klic;
        private int priznak;    // priznak vyvazenosti, pripustne hodnoty pro kazdy vrchol avl stromu jsou -1,0,1
        private bool obsazeno;  // umoznuje vytvorit "prazdne" vrcholy
        private bool hlava;     // specialni oznaceni prvniho vrcholu stromu, do ktereho se neuklada a ma vzdy pouze praveho syna - koren
        private AVLstrom levy_syn;
        private AVLstrom pravy_syn;
        private AVLstrom rodic;

        public int Klic { get { return klic; } }
        public AVLstrom Levy_syn { get { return levy_syn; } }
        public AVLstrom Pravy_syn { get { return pravy_syn; } }
        public AVLstrom Rodic { get { return rodic; } }

        public AVLstrom()       //jediny verejny konstruktor, vytvori hlavu stromu
        {
            this.klic = Int32.MinValue;
            this.obsazeno = true;
            this.hlava = true;
        }

        private AVLstrom(AVLstrom rodic)    //interni konstruktor pouzity pri vkladani novych vrcholu funkci Insert
        { 
            this.klic = 0;
            this.obsazeno = false;
            this.rodic = rodic;
            this.hlava = false;
        }
        
        static private void RotateLeft(AVLstrom uzel)   //rotace doleva, prepsani priznaku
        {
            AVLstrom pom = uzel.pravy_syn;
            
            //prepojeni odkazu
            if (uzel.rodic != null)
            {
                if (uzel.rodic.klic > uzel.klic)
                    uzel.rodic.levy_syn = pom;        
                else
                    uzel.rodic.pravy_syn = pom;
            }
            pom.rodic = uzel.rodic;
            uzel.pravy_syn = pom.levy_syn;
            if (pom.levy_syn != null)
                pom.levy_syn.rodic = uzel;   
            pom.levy_syn = uzel;
            uzel.rodic = pom;
            
            // prepsani priznaku, osetreni vsech realnych situaci (jina moznost by byla pamatovat si hloubky a priznaky znovu pocitat)
            if      ((uzel.priznak == 2) && (pom.priznak == 1))        { uzel.priznak = 0; pom.priznak = 0; }
            else if ((uzel.priznak == 2) && (pom.priznak == -1))       { uzel.priznak = 1; pom.priznak = -2; }
            else if ((uzel.priznak == 2) && (pom.priznak == 2))        { uzel.priznak = -1; pom.priznak = 0; }
            else if ((uzel.priznak == 2) && (pom.priznak == 0))        { uzel.priznak = 1; pom.priznak = -1; }
            else if ((uzel.priznak == 1) && (pom.priznak == 1))        { uzel.priznak = -1; pom.priznak = -1; }
            else if ((uzel.priznak == 1) && (pom.priznak == -1))       { uzel.priznak = 0; pom.priznak = -2; }
            else if ((uzel.priznak == 1) && (pom.priznak == 0))        { uzel.priznak = 0; pom.priznak = -1; }
            
        }

        static private void RotateRight(AVLstrom uzel)
        {
            AVLstrom pom = uzel.levy_syn;
            if (uzel.rodic != null)
            {
                if (uzel.rodic.klic > uzel.klic)
                    uzel.rodic.levy_syn = pom;
                else
                    uzel.rodic.pravy_syn = pom;
            } 
            pom.rodic = uzel.rodic;
            uzel.levy_syn = pom.pravy_syn;
            if (pom.pravy_syn != null)           
                pom.pravy_syn.rodic = uzel;
            pom.pravy_syn = uzel;
            uzel.rodic = pom;

            if      ((uzel.priznak == -2) && (pom.priznak == -1))       { uzel.priznak = 0; pom.priznak = 0; }
            else if ((uzel.priznak == -2) && (pom.priznak == 1))        { uzel.priznak = -1; pom.priznak = 2; }
            else if ((uzel.priznak == -2) && (pom.priznak == -2))       { uzel.priznak = 1; pom.priznak = 0; }
            else if ((uzel.priznak == -2) && (pom.priznak == 0))        { uzel.priznak = -1; pom.priznak = +1; }
            else if ((uzel.priznak == -1) && (pom.priznak == -1))       { uzel.priznak = 1; pom.priznak = 1; }
            else if ((uzel.priznak == -1) && (pom.priznak == 1))        { uzel.priznak = 0; pom.priznak = 2; }
            else if ((uzel.priznak == -1) && (pom.priznak == 0))        { uzel.priznak = 0; pom.priznak = 1; }
            
        }

        public bool Insert(int klic)        //vraci true, pokud hloubka vzrostla
        {
            if (this.obsazeno == false)     //"prazdy" vrchol - tedy list, ktery existuje, ale nema dosazenou hodnotu
            {
                this.klic = klic;
                this.obsazeno = true;
                return true;                //hloubka vzrostla
            }
            else        //obsazeny vrchol, hledame v patricnem podstrome
            {
                switch (klic.CompareTo(this.klic))
                {
                    case 1:
                        {
                            if (pravy_syn == null)
                                pravy_syn = new AVLstrom(this);  //vytvoreni praveho syna s odkazem na aktualni vrchol jako rodice
                            if (pravy_syn.Insert(klic))         //pokracujeme rekurzivne pravym podstromem, hloubka vzrostla
                                {
                                    if (this.hlava == true)
                                        break; //hlavu neni treba vyvazovat, je to jen pomocna konstrukce
                                    switch (this.priznak)
                                    {
                                        case 1:  //uzel mel vetsi pravy podstrom, v nem pribyl vrchol, tedy priznak je nyni +2 -> je treba rotovat
                                            {
                                                this.priznak++;
                                                if (this.pravy_syn.priznak == 1)  //rotace doleva
                                                    RotateLeft(this);
                                                else if (this.pravy_syn.priznak == -1) //dvojrotace
                                                {
                                                    RotateRight(this.pravy_syn);
                                                    RotateLeft(this);
                                                }           
                                                //jina situace nastat nemuze, kdyby priznak praveho syna byl 0, insert by vratil false

                                                return false; //po rotovani je strom vyvazen a hloubka se dale nemeni
                                            }
                                        case 0:
                                            {
                                                this.priznak++; //uzel mel priznak 0 (vyvazeny), vyska praveho podstromu se zvysila, ale vyvazovat neni treba, jen posleme signal o zvyseni dale
                                                return true;
                                            }
                                        case -1:
                                            {
                                                this.priznak++; //uzel mel priznak -1, pridanim vrcholu v pravem podstrome se vyvazil, vyska se nezmenila
                                                return false;
                                            }
                                        default:
                                            break;
                                    }
                                }
                            break;
                        }
                    case -1:
                        {
                            if (levy_syn == null) 
                                levy_syn = new AVLstrom(this);
                            if (levy_syn.Insert(klic))
                                {
                                    if (this.hlava == true)
                                        break;
                                    switch (this.priznak)
                                    {

                                        case -1:  //uzel mel vetsi levy podstrom, v nem pribyl vrchol, tedy priznak je nyni -2 -> je treba rotovat
                                            {
                                                this.priznak--;
                                                if (this.levy_syn.priznak == -1)  //rotace doprava
                                                    RotateRight(this);
                                                else if (this.levy_syn.priznak == 1) //dvojrotace
                                                {
                                                    RotateLeft(this.levy_syn);
                                                    RotateRight(this);
                                                }

                                                return false; //po vyvazeni se hloubka dale nemeni
                                            }
                                        case 0:
                                            {
                                                this.priznak--; //uzel mel priznak 0 (vyvazeny), vyska leveho podstromu se zvysila, ale vyvazovat neni treba, jen posleme signal o zvyseni dale
                                                return true;
                                            }
                                        case 1:
                                            {
                                                this.priznak--; //uzel mel priznak 1, pridanim vrcholu v levem podstrome se vyvazil, vyska se nezmenila
                                                return false;
                                            }
                                        default:
                                            break;
                                    }
                                    
                                }
                            break;
                        }
                    case 0:
                            break; //klic uz ve strome je, nevklada se
                    default:
                        break;
                }
            }
            return false;
        }

        public bool Delete(int klic) // vraci true, pokud hloubka klesla
        {
            if (this.hlava)
            {
                if (this.pravy_syn == null)
                    return false;
                else
                    return this.pravy_syn.Delete(klic);
            }
            switch (klic.CompareTo(this.klic))  //hledame mazany vrchol
            {
                case 1:
                    {
                        if (pravy_syn == null)
                            break;      //klic ve strome neni, nemaze se
                        if (pravy_syn.Delete(klic))     //klic ve strome je, funkce je volana rekurzivne na pravy podstrom
                            {
                                //hloubka se zmenila (klesla)
                                switch (this.priznak)
                                {
                                    case 1:
                                        {
                                            priznak--;      //podstrom se vyvazil, ale hloubka je nizsi -> posila se signal vyse
                                            return true;
                                        }
                                    case 0:
                                        {
                                            priznak--;
                                            return false;   // podstrom byl vyvazeny, smazani hodnoty v pravem podstrome nezmenilo hloubku
                                        }
                                    case -1:                // novy priznak -2, vyvazujeme
                                        {
                                            priznak--;
                                            switch (levy_syn.priznak)
                                            {
                                                case -1:
                                                    {
                                                        RotateRight(this);      //rotujeme doprava, podstrom se zmensi
                                                        return true;        
                                                    }
                                                case 0:
                                                    {
                                                        RotateRight(this);      //rotujeme doprava, ale podstrom se nezmensuje 
                                                        return false;
                                                    }
                                                case 1:
                                                    {
                                                        RotateLeft(this.levy_syn);      // dvojrotace, podstrom se zmensi
                                                        RotateRight(this);
                                                        return true;
                                                    }
                                                default:
                                                    return false;
                                            }
                                        }
                                    default:
                                        break;
                                }
                            }
                        break;
                    }
                case -1:
                    {
                        if (levy_syn == null)
                            break;
                        if (levy_syn.Delete(klic)) 
                        {
                                switch (this.priznak)
                                {
                                    case -1:
                                        {
                                            priznak++;
                                            return true;
                                        }
                                    case 0:
                                        {
                                            priznak++;
                                            return false;
                                        }
                                    case 1:    
                                        {
                                            priznak++;
                                            switch (pravy_syn.priznak)
                                            {
                                                case 1:
                                                    {
                                                        RotateLeft(this);
                                                        return true;
                                                    }
                                                case 0:
                                                    {
                                                        RotateLeft(this);
                                                        return false;
                                                    }
                                                case -1:
                                                    {
                                                        RotateRight(this.pravy_syn);
                                                        RotateLeft(this);
                                                        return true;
                                                    }
                                                default:
                                                    return false;
                                            }
                                        }
                                    default:
                                        break;
                                }
                        }
                        break;
                    }
                case 0:         //nasli jsme spravny vrchol, jdeme mazat
                    {
                        if ((this.pravy_syn == null) && (this.levy_syn == null))  //vrchol je list, muzeme bez problemu smazat
                        {
                            if (this.klic < rodic.klic)     //prepojeni rodice
                                rodic.levy_syn = null;
                            else
                                rodic.pravy_syn = null;
                            return true;
                        }
                        else if (this.pravy_syn == null)        // vrchol ma pouze leveho syna, staci jej prepojit na misto mazaneho vrcholu
                        {
                            if (this.klic < rodic.klic)
                            {
                                rodic.levy_syn = this.levy_syn;
                                this.levy_syn.rodic = this.rodic;
                            }
                            else
                            {
                                rodic.pravy_syn = this.levy_syn;
                                this.levy_syn.rodic = this.rodic;
                            }
                            return true;
                        }
                        else if (this.levy_syn == null)         // vrchol ma pouze praveho syna
                        {
                            if (this.klic < rodic.klic)
                            {
                                rodic.levy_syn = this.pravy_syn;
                                this.pravy_syn.rodic = this.rodic;
                            }
                            else
                            {
                                rodic.pravy_syn = this.pravy_syn;
                                this.pravy_syn.rodic = this.rodic;
                            }
                            return true;
                        }
                        else                // vrchol ma oba syny, je treba najit nahradu v podstrome a prohodit klice
                        {
                            AVLstrom nahrada;
                            if (this.priznak == -1)         // nahradu bereme ve vetsim podstrome, vyhneme se pripadnemu vyvazovani
                                nahrada = levy_syn.NajdiMax();    
                            else
                                nahrada = pravy_syn.NajdiMin();

                            int nahradniklic = nahrada.klic;        // zapamatujeme si hodnotu nalezeneho substituenta
                            
                            bool navrat = this.Delete(nahrada.klic);   // nahradni vrchol smazeme (ma jiste nejvyse jednoho syna, tedy nastane jedna ze situaci vyse)
                            this.klic = nahradniklic;               // mazany klic nahradime klicem smazaneho substituenta
                            
                            return navrat;                          //mazani v podstrome mohlo zmenit hloubku, signal zmeny propagujeme vyse
                        }
                    }
                default:
                    break;
            }
            return false;
        }
        
        private AVLstrom NajdiMax()
            {
                if (pravy_syn == null)
                    return this;
                else
                    return this.pravy_syn.NajdiMax();
            }

        private AVLstrom NajdiMin()
        {
            if (levy_syn == null)
                return this;
            else
                return this.levy_syn.NajdiMin();
        }

        public string Vypis()       //funkce vraci vypis ve formatu string, kde patra stromu jsou ve sloupeccich a synove vpravo od otce, pravy syn prvni
        {
            StringBuilder vypis = new StringBuilder();
            AVLstrom vrchol;
            if (this.hlava)
                vrchol = this.pravy_syn;
            else
                vrchol = this;
            if (vrchol == null)
            {
                return "";
            }
            CyklusVypisu(vrchol, ref vypis, 0); //rekurzivni funkce projde strom a do stringbuilderu dosadi klice a priznaky vrcholu

            vypis.Append('\n');
            return vypis.ToString();
        }

        private void CyklusVypisu(AVLstrom vrchol, ref StringBuilder vypis, int hloubka)
        {
            char priznak;
            switch (vrchol.priznak)     //reprazentace priznaku +,- nebo °
            {
                case 0: { priznak = '°'; break; }
                case -1: { priznak = '-'; break; }
                case 1: { priznak = '+'; break; }
                default: { priznak = ' '; break; }
            }

            vypis.Append(vrchol.klic.ToString() + priznak + "\t");      //spravny format se zachova pro klice < 1000000

            if (vrchol.pravy_syn != null)
                CyklusVypisu(vrchol.pravy_syn, ref vypis, hloubka + 1);
            if (vrchol.levy_syn != null)
            {
                vypis.Append("\n");
                vypis.Append('\t', hloubka + 1);        //levy syn se zarovna pod praveho
                CyklusVypisu(vrchol.levy_syn, ref vypis, hloubka + 1);
            }
        }
       
        public bool Find(int klic)  //vraci true, pokud je klic ve strome
        {
            if (this == null)
                 return false;      // klic ve strome neni

            if (this.hlava)
            {
                if (this.pravy_syn == null)
                    return false;
                else
                    return this.pravy_syn.Find(klic);
            }
            else
            {
                switch (klic.CompareTo(this.klic))
                {
                    case 0: return true;            //nasli jsme klic
                    case 1:
                        {
                            if (this.pravy_syn == null)
                                return false;
                            else
                                return this.pravy_syn.Find(klic);
                        }
                    case -1:
                        {
                            if (this.levy_syn == null)
                                return false;
                            else
                                return this.levy_syn.Find(klic);
                        }
                    default: return false;
                }
            }
            
        }
    }
}
