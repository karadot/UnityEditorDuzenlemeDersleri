using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//Bu kısımda CustomEditor özelliği ile hangi sınıfımız için bu editoru yazdığımızı belirtiyoruz.
[CustomEditor (typeof (KarakterVeri))]
//Editor sınıflarımızda MonoBehaviour değil Editor kullanırız.
public class KarakterVeriEditor : Editor {

    //Yaratacağımız veriyi tutmak için kullanacağımız değişken
    KarakterVeri veriler;

    //Bu iki değişken ile arayüzde genişlik olarak yarım ve üçte bir alanlar kullanacağız.
    GUILayoutOption yarim;
    GUILayoutOption uctebir;

    //Görsellik için ayarları tuttuğumuz değişken
    static GUISkin karakterveriSkin;

    //İsim ve soyisim verilerini önceden belirliyoruz ve sonrasında bunlar içerisinden rastgele seçim yapacağız.
    string[] names = new string[] {
        "Fannie",
        "Rebecca ",
        "Khadija",
        "Ffion",
        "Chantelle",
        "Kane",
        "Laura",
        "Milly",
        "Michelle",
        "Caitlyn",
        "Alistair",
        "Luke",
        "James",
        "Haris",
        "Ibrahim",
        "Abdullah",
        "Kyle",
        "Abdul",
        "Laurence",
        "Liam"
    };

    string[] surnames = new string[] {
        "Lamb",
        "Evans",
        "Alexander",
        "Rowe",
        "Ford",
        "Paul",
        "Turner",
        "Peters",
        "Wang",
        "Davis"
    };

    //İsim ve soyisim verilerini rastgele seçmemizi sağlar.
    void RastgeleIsim () {
        string isim = names[Random.Range (0, names.Length - 1)];
        string soyisim = surnames[Random.Range (0, surnames.Length - 1)];

        veriler.isim = isim;
        veriler.soyisim = soyisim;
    }

    //Oluşturacağımız paneldeki arayüz görünümünü düzenlediğimiz fonksiyon
    public override void OnInspectorGUI () {

        //Yarattığımız skin dosyasını arayüzümüze giydiriyoruz.
        GUI.skin = karakterveriSkin;
        GUI.skin.font = karakterveriSkin.font;

        /*
        Arayüz elemanlarımızı dikey halde sıralamak için BeginVertical komutunu kullanıyoruz.
        Yani bu komut sonrasındaki her ayrı eleman birbirinin altına gelecek şekilde sıralanacak.
        */
        GUILayout.BeginVertical ();
        /*
        Bu satırlarda panelimizin varolan genişliğine göre yarım ve uctebir boyutlarını hesaplıyoruz.
        30 birimlik çıkarma yapmamızın sebebi de aralarında bir boşluk oluşturmak, kenarlara yapışık durmasını engellemek.
        */
        yarim = GUILayout.Width ((EditorGUIUtility.currentViewWidth / 2) - 30);
        uctebir = GUILayout.Width ((EditorGUIUtility.currentViewWidth / 3) - 10);

        /*
        Burada farklı fonksiyonları çağırdığımı göreceksiniz. Hem görsel olarak hem de düzenleme olarak
        kolaylık sağlaması amacıyla yaratacağım arayüzü bölümlere ayırdım ve farklı fonksiyonlarda yarattım.
        */
        IsimKismi ();
        //Space komutu ile boşluk ekliyoruz. 30 birimlik bir boşluk ekledik burada.
        GUILayout.Space (30);
        //Label komutu ile metin ekliyoruz. Özellikler alanı için bir başlık olarak koydum.
        GUILayout.Label ("Özellikler");
        /*
        BeginVertical komutuna benzer şekilde sıralamayı düzenlediğimiz bir komut BeginHorizontal komutu
        Ancak farklı olarak dikey yerine yatay sıralama yapmamızı sağlıyor.
        */
        GUILayout.BeginHorizontal ();
        SaglikKismi ();
        ManaKismi ();
        GucKismi ();
        //Yatay sıralama bölümünü bitiriyoruz.
        GUILayout.EndHorizontal ();
        TaslaklarKismi ();
        //Dikey sıralamayı bitiriyoruz.
        GUILayout.EndVertical ();

        /*
        Eğer bu yarattığımız arayüzde bir değişiklik olduysa, verileri düzenlediysek
        Bu iki komut sayesinde bunları Unity'e bildiriyoruz. Bunu yapmazsak save işlemi yaptığımızda
        oluşturduğumuz veriler kaydolmayacaktır.
        */
        if (GUI.changed) {
            EditorUtility.SetDirty (veriler);
            EditorSceneManager.MarkSceneDirty (veriler.gameObject.scene);
        }
    }

    //Taslaklar kısmını oluşturmamızı sağlar.
    void TaslaklarKismi () {
        //Taslaklar bölümünün başladığını belirtiyorum.
        GUILayout.Label ("Taslaklar");
        /*
        Yatay şekilde yani yanyana sıralama yaptırmak istiyorum.
        Bu yüzden BeginHorizontal komutunu kullandım.
        */
        GUILayout.BeginHorizontal ();
        /*
        Bu kısımda 3 adet button oluşturuyoruz. If blogu olmadan da 
        butonları oluşturabiliriz. Ancak if komutu içerisine koyduğumuzda
        hem ekrana yazdırıyor hem de tıklanma durumlarını kontrol ediyoruz.
        Button içerisine verdiğim ilk değişken yazacak metin iken, ikinci değişken de
        arayüz elemanının ölçülerini belirtiyor. Burada tek bir değişken verdim
        ancak süslü parantez "{}" içerisine birden fazla değişken yazarak da ekleyebilirsiniz.
        Veya GUILayoutOption[] şeklinde dizi oluşturup boyut özelliklerini bu diziye aktarabilir
        ve bu diziyi değişken olarak verebilirsiniz.
        */
        if (GUILayout.Button ("Tank", uctebir)) {
            Tank ();
        }
        if (GUILayout.Button ("Büyücü", uctebir)) {
            Buyucu ();
        }
        if (GUILayout.Button ("Suikastçi", uctebir)) {
            Suikastci ();
        }
        //Yatay sıralama sonu sıralama sonu
        GUILayout.EndHorizontal ();
    }

    //Karakterin isim soyisim verileriyle alakalı arayüz
    void IsimKismi () {
        GUILayout.Label ("KARAKTER");
        GUILayout.BeginHorizontal ();
        GUILayout.BeginVertical ();
        GUILayout.Label ("İsim", yarim);
        /*Bu kısımda daha önce bahsetmediğim tek şey TextField elemanı
        Bu eleman sayesinde bir metin alanı oluşturuyoruz ve bundan veri alabiliyoruz.
        İlk değişkenimiz metin alanı içerisinde yazacak olan metin, ikincisi ise arayüz elemanının
        ölçü ayarı.
        Dikkat ettiyseniz = komutu ile veriler.isim değişkenimize de atama yapıyoruz. 
        Burada bunu yapma amacımız, yarattığımız metin alanında bir değişiklik yapıldığında 
        bunu değişkenimize yansıtabilmek.
        */
        veriler.isim = GUILayout.TextField (veriler.isim, yarim);
        GUILayout.EndVertical ();
        GUILayout.BeginVertical ();
        GUILayout.Label ("Soyisim", yarim);
        veriler.soyisim = GUILayout.TextField (veriler.soyisim, yarim);
        GUILayout.EndVertical ();
        GUILayout.EndHorizontal ();

        if (GUILayout.Button ("Rastgele İsim")) {
            RastgeleIsim ();
        }
    }

    /*
    Sağlık, mana ve güç kısımları benzer formatta olduğu için
    sadece sağlık fonksiyonu içerisinde açıklamalar ekledim.
    Diğer ikisinde sadece verilen değişkenler ve arayüz düzenlemeleri farklı.
    Aslında bir struct oluşturup bu 3 tipin kendine has verilerini değişken olarak tutarak
    tek bir fonksiyona değişkenlerini vererek yapmak daha mantıklı. Size ufak bir egzersiz :)
    */
    void SaglikKismi () {
        GUILayout.BeginVertical ();
        /*
        GUIStyle tipindeki değişkenlerle arayüzümüzün font, padding gibi 
        özelliklerini değiştirebiliyoruz. Burada da yaptığımız şey, 
        daha önce oluşturduğumuz karakterveriSkin dosysından OzellikLabel isimli
        stil alanını almak.
        ARdından da arkaplanına bir renk ataması yapıyoruz.
        */
        GUIStyle labelStyle = karakterveriSkin.GetStyle ("OzellikLabel");
        labelStyle.normal.background = ColorTex (Color.red);

        //IconContent komutu ile ismini verdiğimiz dosyaya erişip bunun resim verisini elde ediyoruz.
        Texture image = EditorGUIUtility.IconContent ("health").image;
        /*
        GUIContent ise görsel içeriğimizin verilerini oluşturmak için kullandığımız bir değişken türü.
        Burada 2 adet değişken verdik, bunlar metin ve resim değişkenleri. Farklı değişkenler alabilen 
        override hali de bulunuyor.
        */
        GUIContent labelContent = new GUIContent ("Sağlık", image);
        /*
        Burada daha önce yaptıklarımızdan farklı olarak label içerisinde direkt metin yazmak yerine
        yarattığımız içeriği, stili ve boyut özelliğini veriyoruz.
        */
        GUILayout.Label (labelContent, labelStyle, uctebir);
        GUILayout.Space (5);
        /*
        Metin alanında olduğu gibi, kullanıcıdan veri alabileceğimiz bir alan oluşturuyoruz ancak 
        bu string yerine float tipinde değer döndürüyor. Çalışma mantıkları da aynı şekilde,
        hem değişkeni verip hem de değişken atamasını = ile yapıp veri alabiliyoruz.
        */
        veriler.saglik = EditorGUILayout.FloatField (veriler.saglik, GUI.skin.textField, uctebir);
        GUILayout.EndVertical ();
    }

    void ManaKismi () {
        GUILayout.BeginVertical ();
        GUIStyle labelStyle = karakterveriSkin.GetStyle ("OzellikLabel");
        labelStyle.normal.background = ColorTex (Color.blue);
        Texture image = EditorGUIUtility.IconContent ("mana").image;
        GUIContent labelContent = new GUIContent ("Mana", image);
        GUILayout.Label (labelContent, labelStyle, uctebir);
        GUILayout.Space (5);
        veriler.mana = EditorGUILayout.FloatField (veriler.mana, GUI.skin.textField, uctebir);
        GUILayout.EndVertical ();
    }
    void GucKismi () {
        GUILayout.BeginVertical ();
        GUIStyle labelStyle = karakterveriSkin.GetStyle ("OzellikLabel");
        labelStyle.normal.background = ColorTex (Color.green);
        Texture image = EditorGUIUtility.IconContent ("stamina").image;
        GUIContent labelContent = new GUIContent ("Güç", image);
        GUILayout.Label (labelContent, labelStyle, uctebir);
        GUILayout.Space (5);
        veriler.guc = EditorGUILayout.FloatField (veriler.guc, GUI.skin.textField, uctebir);
        GUILayout.EndVertical ();
    }

    /*
    Burada obje üzerinde skin sıfırlamak için bir buton oluşturuyoruz. Ancak bu buton KarakterVeri
    bileşenin sağ üstünde bulunan context menü içerisinde olacak.
    */
    [MenuItem ("CONTEXT/KarakterVeri/Temel Skine Dön")]
    static void temelSkin () {
        if (karakterveriSkin != null) {
            karakterveriSkin.textField = new GUIStyle (EditorStyles.textField);
            karakterveriSkin.button = new GUIStyle (EditorStyles.miniButtonMid);
            karakterveriSkin.label = new GUIStyle (EditorStyles.label);
        }
    }

    //Verilen rentkte 1 piksellik bir texture oluşturur.
    Texture2D ColorTex (Color color) {
        Texture2D texture = new Texture2D (1, 1);
        texture.SetPixel (1, 1, color);
        texture.Apply ();
        return texture;
    }

    /*
    OnEnable komutu ile KarakterVeri scriptine sahip bir obje seçili olduğunda 
    karakterin verileri çekiyoruz ve kendi veriler değişkenimize atıyoruz.
    Burada target seçili olan objeyi temsil ediyor. Bu sayede her farklı obje seçtiğimizde
    verilerimizi o objedeki bileşenimizden alıp işleme sokacağız.
     */
    void OnEnable () {
        veriler = (KarakterVeri) target;
        if (karakterveriSkin == null)
            karakterveriSkin = EditorGUIUtility.Load ("KarakterVeriSkin.guiskin") as GUISkin;

    }

    /*
    Burada taslak amacıyla 3 farklı tipte karakter için
    statlar oluşturuyoruz. Bunları bu şekilde elle girmek yerine 
    farklı bir yapı da kurabilirsiniz.
    */
    void Tank () {
        veriler.saglik = 20;
        veriler.mana = 3;
        veriler.guc = 7;
    }

    void Buyucu () {
        veriler.saglik = 9;
        veriler.mana = 20;
        veriler.guc = 1;
    }

    void Suikastci () {
        veriler.saglik = 7;
        veriler.mana = 2;
        veriler.guc = 20;
    }
}
