using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelTasarlamaAraci : EditorWindow {

    /*
    Tasarımda kullanacağımız prefabların bulunduğu klasörleri
    tutacağımız değişkenler
    */
    static List<List<string>> klasorPrefablari = new List<List<string>> ();
    static string[] altKlasorler;

    /*
    Oluşturacağımız arayüzde hangi prefab klasörünün
    seçili halde olduğu bilgisini tuttuğumuz değişken.
    */
    static int seciliKlasor = 0;

    static bool sahnePaneliniAc = false;
    static Vector2 sahnePaneliPos = Vector2.zero;

    /*
    ScrollView yani aşağı yukarı kaydırabildiğimiz panellerimizde
    scroll değerini tutmak için kullandığımız değişkenler.
    */
    static Vector2 prefabPanelScroll = Vector2.zero;
    static Vector2 klasorPanelScroll = Vector2.zero;

    //Sahnedeki kamera verisi için oluşturduğumuz değişken
    static Camera sahneCamera;

    static bool altObjeOlarakEkle = false;
    static bool seciliHaldeOlustur = false;
    static bool mousePanelAc = false;

    /*
    Prefab Klasörü adresini tutacağımız değişken
    Boş bir değer vermemek adına, ilk başta veriyi elle giriyorum.
    */
    static string prefabKlasoru = "Assets/Prefabs";

    /*
    Oluşturduğumuz araç panelini aktif hale getirmek için
    MenuItem özelliğini kullanarak menüye yeni bir buton ekliyoruz
    */
    [MenuItem ("Editor Dersleri/Level Aracı")]
    static void ShowWindow () {
        //LevelTasarlamaAraci Tipindeki paneli, pencereyi oluşturup aktif hale getiriyoruz.
        var window = GetWindow<LevelTasarlamaAraci> ();
        window.titleContent = new GUIContent ("LevelTasarlamaAraci");
        window.Show ();
    }

    //Level aracımızın ayarlarının olduğu panel arayüzü
    void OnGUI () {
        //Label ile başlıklar ekliyoruz
        GUILayout.Label ("Karadot Level Oluşturma Aracı");
        GUILayout.Label ("Seçenekler");

        /*
        Farklı seçeneklerimizi açıp kapatabileceğimiz arayüz elemanlarını ekliyoruz.
        Toggle elemanı true veya false değer elde ederiz.
        Alacağı ilk değişken true-false değerini tutan değişken, ikincisi ise metin kutusunda yazacak yazıdır.
        bunu = ifadesi kullanarak atadığımızda ise, Toggle elemanından gelecek true-false değeri de bu değişkene atayabiliyoruz.
        */
        altObjeOlarakEkle = GUILayout.Toggle (altObjeOlarakEkle, "Alt Obje Olarak Ekle");
        seciliHaldeOlustur = GUILayout.Toggle (seciliHaldeOlustur, "Seçili Halde Oluştur");
        mousePanelAc = GUILayout.Toggle (mousePanelAc, "Mouse Konumunda Panel Aç");

        GUILayout.Label ("Seçili Klasör: " + prefabKlasoru);
        /*
        Bu kısım biraz karmaşık gelebilir. 
        Normalde Button elemanını if bloguna yazmadan da kullanabiliyoruz.
        Ancak if blogunun içerisine eklediğimizde, bu buton tıklanırsa 
        ne yapılmasını istediğimizi belirtebiliyoruz.
        */
        if (GUILayout.Button ("Prefab Klasörü Seç")) {
            PrefabSecmeEkrani ();
        }

        //Eğer prefabKlasörü seçili değilse, boşta ise, bir dosya yolu belirtmiyorsa uyarı oluşturuyoruz.
        if (prefabKlasoru == "") {
            /*
            HelpBox sayesinde bir hatırlatma, yardım görünümü oluşturuyoruz ve kullanıcıyı  bilgilendirebiliyoruz.
            Alacağı ilk değişken mesajımız iken, ikinci değişken mesajımızın önem durumunu belirten MessageType. 
            "Warning"  yerine "Error" ve "Info" da kullanabilirsiniz
            */
            EditorGUILayout.HelpBox ("Prefab klasörü assets altında olmalıdır", MessageType.Warning);
        }

        GUILayout.Label ("Seçilen Alt Klasörler");
        //Seçili olan prefab klasörlerinin altında olan klasörleri sırayla dolanıp ismini ve kaç adet prefaba sahip olduğunu panele yazdırıyoruz.
        for (int i = 0; i < altKlasorler.Length; i++) {
            GUILayout.Label (altKlasorler[i] + " klasörü " + klasorPrefablari[i].Count + " adet prefab Bulunduruyor");
        }
    }
    //Panel penceremiz aktif hale gelince PaneliYukle ile gerekli verileri çekiyoruz.
    void OnEnable () {
        PaneliYukle ();
    }

    void OnDisable () {
        /*
        Panelimiz kapandığında Scene paneli aktifken çağırılan 
        listener olayından fonksiyonumuzu siliyoruz.
        */
        SceneView.duringSceneGui -= OnSceneView;
    }
    void OnInspectorUpdate () {
        PaneliYukle ();
    }

    void PaneliYukle () {
        /*
        Burada listener olayından kaydımızı önce silip sonra tekrar aktif hale getiriyoruz.
        Bunu yapma amacımız bir noktada bizden bağımsız şekilde listener kaydı silinememişse,
        tekrar atama yapmadan önce kaydı sildiğimizden emin olmak.
        */
        SceneView.duringSceneGui -= OnSceneView;
        SceneView.duringSceneGui += OnSceneView;

        /*
        Sahnede varolan kameraları bir diziye alıyoruz
        */
        Camera[] kameralar = SceneView.GetAllSceneCameras ();

        //Eğer hiçbir kamera bulunamazsa bir hata bildirimi oluşturuyoruz ve hiçbir şey yapmadan geri döndürüyoruz.
        if (kameralar == null || kameralar.Length == 0) {
            Debug.LogWarning ("Kamera boş");
            return;
        } else {
            //Eğer kamera bulunduysa ilk kamerayı alıyoruz. Bu kamera Scene Panelinde görüntü sağlayan kameradır.
            sahneCamera = kameralar[0];
        }

        //Kameraya erişebildiysk prefabları göstereceğimiz fonksiyonu çağırıyoruz.
        PrefablariYukle ();
    }

    //Level aracımızın Scene paneli içerisinde oluşturacağımız arayüz
    void OnSceneView (SceneView scene) {
        //eğer sahne kamerasını bulamadıysa hiçbir şey yapmadan geri dönüyoruz.
        if (sahneCamera == null)
            return;
        /*
        Handles.BeginGUI ve Handles.EndGUI fonksiyonları Scene panelinde 
        kendi yazacağımız arayüzü göstermek için kullanmamız gereken zorunlu kodlar.
        Bu iki komut arasına yazacağımız arayüz komutları Scene panelinde gözükecektir.
        */
        Handles.BeginGUI ();
        /*
        Scene panelinin sol alt köşesine yazı yazdırmak için bu Label komutunu kullanıyoruz.
        Aldığı ilk değişken Rect tipinde olacak. UI ile uğraştıysanız Rect Transform isimli bileşene aşinasınızdır.
        Rect tipi 4 adet değişkene ihtiyaç duyuyor, X konumu - Y konumu - Genişlik - Yükseklik.
        Label için 2.değişken ise yazdırmak istediğimiz yazı
        3. değişken de stil ayarı. Burada toolbarButton kullanmayı tercih ettim.
        Siz de farklı stiller deneyebilirsiniz.
        */
        GUI.Label (new Rect (sahneCamera.scaledPixelWidth - 150, sahneCamera.scaledPixelHeight - 20, 150, 20), "Karadot Sahne Aracı", EditorStyles.toolbarButton);
        //Eğer sahnePaneliniAc değişkenimiz true değere sahipse prefabları gösterdiğimiz paneli açıyoruz.
        if (sahnePaneliniAc) {
            PrefabPaneli ();
        }
        Handles.EndGUI ();

        /*
        Event tipi burada  etkileşimlerini, olaylarını kontrol için kullandığımız bir tip.
        Bu sayede tıklama, tuşa basma durumlarını kontrol edebiliyoruz.
        Event.current ile o an bir işlem yapıldıysa bunun bilgisini elde ediyoruz.
        */
        Event e = Event.current;

        /*
        Burada switch kullanma sebebim tamamen örnek amaçlı. İf bloguyla da deneyebilirsiniz.
        */
        switch (e.type) {
            //Eğer herhangi bir tuş basılmayı bırakıldıysa
            case EventType.KeyUp:
                //eğer basılmayı bırakılan tuş Tab tuşu ise
                if (e.keyCode == KeyCode.Tab) {
                    /*
                    Sahne panelini varolanın tersi hale getiriyoruz.
                    Yani true ise false, false ise true hale geliyor.
                    */
                    sahnePaneliniAc = !sahnePaneliniAc;

                    //Eğer mouse konumunda panelin açılmasını istiyorsak, true değerde ise
                    if (mousePanelAc) {
                        //Sahne kamerasını baz alarak, farenin konumunu elde ediyoruz.
                        Vector2 geciciPos = sahneCamera.ScreenToViewportPoint (Event.current.mousePosition);
                        /*
                         x ve y konumlarını kontrol ediyoruz.
                         Scene paneline göre fare konumun aldığımız için panelin içerisinde mi,
                         yoksa panelin sınırları dışında mı bunu kontrol ediyoruz.
                         Panelin sol üstü (0,0) iken sağ altı (1,1) değerlerine sahiptir.
                         */
                        if (geciciPos.x > 0 && geciciPos.x < 1 && geciciPos.y > 0 && geciciPos.y < 1) {
                            sahnePaneliPos = sahneCamera.ViewportToScreenPoint (geciciPos);
                        } else {
                            sahnePaneliPos = Vector2.zero;
                        }
                    }
                }
                break;
        }
    }

    void PrefabPaneli () {
        //Yaratacağımız arayüz için stillendirme oluşturuyoruz
        GUIStyle areaStyle = new GUIStyle (GUI.skin.box);
        areaStyle.alignment = TextAnchor.UpperCenter;

        //Oluşturacağımız panelin ölçülerini ve konum bilgisini tutmak için bir Rect değişkeni oluşturduk.
        Rect panelRect;
        //Eğer mouse konumunda açmak istiyorsak, daha öncesinde oluşturduğumuz sahnePaneliPos isimli değişkeni kullanıyoruz.
        if (mousePanelAc) {
            panelRect = new Rect (sahnePaneliPos.x, sahnePaneliPos.y, 200, 300);
        } else {
            /*
            Mouse konumunda açılmasını istemediğimiz zamanlarda sol tarafta olmasını istiyorum.
            Bu yüzden Rect değişkeninin ilk 2 değişkeni sıfır, yani sol üstten başlatıyoruz.
            240 birim genişlik, sabit olmasını istediğim için.
            Son değişken ise Scene panelinin yüksekliğini elde etmemizi sağlayan bir kod. Yani
            Scene panelinin yüksekliği ile bizim oluşturduğumuz panelin yüksekliği aynı ölçüde olacak.
            */
            panelRect = new Rect (0, 0, 240, SceneView.currentDrawingSceneView.camera.scaledPixelHeight);
        }

        /*
        BeginArea ve EndArea bir arayüz bölgesi oluşturmak için kullandığımız komut.
        Bu ikisi arasında yazdığımız arayüz bileşenleri BeginArea içinde verdiğimiz 
        ilk değişken olan Rect tipi değişkene göre belirli bir alan içerisinde kalacak.
        2. değişken olarak ise stil değişkeni veriyoruz.
        */
        GUILayout.BeginArea (panelRect, areaStyle);

        /*
        Klasörleri seçilebilir halde tutmak istiyorum ve bunları bir scrollview içerisinde tutarak 
        kaydırılabilir bir panel içerisinde tutacağım.
        BeginScrollView ile aşağı-yukarı, sağa-sola kaydırılabilir bölümler oluşturabilirim.
        Sırasıyla değişkenleri yazarsak
        1.klasorPanelScroll:Kaydırmanın hangi durumda olduğunu gösteriyor, sağ mı sol mu aşağı mı yukarı mı, bunun verisini vector2 olarak tutuyoruz.
        Dikkat ederseniz = ile yine klasorPanelScroll değişkenine atama yapıyoruz. Scrollview üzerinde kaydırma yaptığımızda, bu şekilde verimizi güncelleyebiliyoruz, sabit kalmıyor.
        2. değişken true değere sahip, bu değişkenin karşılığı horizontalScroll aslında, yani sağa sola kaydırma. Ben de bu şekilde olmasını istediğim için true veriyorum.
        3. değişken de bu sefer dikeyde kaydırma durumunu soruyor, false veriyorum. Çünkü yatayda kaydırma olmasını istemiyorum.
        4. değişkende horizontal yani yatay kaydırma çubuğum için istediğim stili veriyorum
        5. değişken de vertical yani dikey kaydırma çubuğu için stil değişkeni, herhangi bir stil atamasını istemiyorum.
        6. değişken de ölçülerde sınırlandırma yapmamızı sağlayan değişken. MinHeight 40 vererek, en az 40 birim yükseklikte olmasını sağlıyorum.
        */
        klasorPanelScroll = GUILayout.BeginScrollView (klasorPanelScroll, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.MinHeight (40));

        /*
        ScrollView içerisine bir adet toolbar koyuyorum. Bu aslında bir sürü butonu içinde barındıran ancak sadece 1 tanesinin aktif-seçili olabildiği bir arayüz yapısı.
        Bunu da hangi alt klasörün seçildiğini tutmak için kullanıyorum. 
        Aldığı ilk değişken int tipinde ID değişkeni
        2.değişken ise içerisine dolduracağımız butonlara yazılacak isimlerin olduğu altKlasörler dizisi.
        Yine dikkat ederseniz, oluşturduğumuz arayüzden veriyi alabilmek için = ile seciliKlasor değişkenine atama yapıyorum.
        */
        seciliKlasor = GUILayout.Toolbar (seciliKlasor, altKlasorler);

        //EndScrollView komutunu çağırmayı unutmayın.
        GUILayout.EndScrollView ();

        /*
        Bu sefer de prefabları göstereceğim bir scrollview oluşturacağım. 
        Bir önceki ScrollView dan farklı olarak bu sefer dikeyde yani vertical halde hareket istiyorum.
        Yine MinHeight kullandım ancak dikkat ederseniz, Area için verdiğim panelRect yüksekliğinden 40 çıkarıyorum.
        Bu sayede bu iki ScrollView birbirine tam olarak oturup ekranı kaplayacaklar.
        Ayrıca bu sefer prefabPanelScroll değişkenini kullandığım dikkatinizden kaçmasın. Her bir scroll için ayrı bir değişkende bu veriyi tutmamız gerekiyor.
        */
        prefabPanelScroll = GUILayout.BeginScrollView (prefabPanelScroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.MinHeight (panelRect.height - 40));

        /*
        Toolbar sayesinde elde ettiğim seçiliKlasor ile sadece istediğimiz klasördeki prefabları
        sırayla dolanıp ekranda bunlar için birer arayüz elemanı oluşturuyorum.
        */
        for (int i = 0; i < klasorPrefablari[seciliKlasor].Count; i++) {
            /*
            Bu kısımda yaptığım şey prefab ismini elde edebilmek için
            bir filtreleme işlemi. Dosya yolu halinde tuttuğum için
            elimize geçen veri "klasorismi/isim.prefab" formatında.
            Burada da sadece isim kısmına erişmek için filtreleme yapıyorum.
            Substring ile en sondaki slash "/" sonrası ve .prefab öncesi kısımları alarak
            isim değerini elde ediyorum.
            */
            int index = klasorPrefablari[seciliKlasor][i].LastIndexOf ("/");
            string isim = "";
            if (index >= 0) {
                isim = klasorPrefablari[seciliKlasor][i].Substring (index + 1, klasorPrefablari[seciliKlasor][i].Length - index - 8);
            } else {
                isim = klasorPrefablari[seciliKlasor][i];
            }

            /*
            Özelleştirilebilir halde bir buton oluşturmak için
            GUIContent tipinde bir değişken oluşturuyorum.
            text değişkenine ismi, image değişkenine ise prefabGorseliAl 
            fonksiyonu ile elde ettiğim resim verisini atıyorum.
             */
            GUIContent icerik = new GUIContent ();
            icerik.text = isim;
            icerik.image = prefabGorseliAl (klasorPrefablari[seciliKlasor][i]);
            /*
            Her bir prefab için ayrı ayrı butonları oluşturuyorum. Dikkat ederseniz Button'da değişken olarak icerik değişkenini kullandım.
            Ve bu butonların tıklanma durumuna da ObjeOlustur isimli fonksiyonu veriyorum.
            */
            if (GUILayout.Button (icerik)) {
                ObjeOlustur (klasorPrefablari[seciliKlasor][i]);
            }
        }
        //Prefablar için olan scrollview elemanını sonlandırıyorum.
        GUILayout.EndScrollView ();
        //Arayüz panelimi sonlandırıyorum.
        GUILayout.EndArea ();
    }

    //Verilen dosya yolundaki prefabı sahnede oluşturmamızı sağlar.
    void ObjeOlustur (string prefabYolu) {
        //Verilen prefabYolu ile dosyayı bulup bir obje değişkenine aktarıyorum.
        Object obj = AssetDatabase.LoadAssetAtPath<GameObject> (prefabYolu);
        /*
        Objeyi InstantiatePrefab komutuyla sahnede yaratıyorum ve bir değişkene atıyorum.
        Oyun içerisinde kullandığımız Instantiate komutundan farklı olarak obje bilgisi dışında
        hangi sahnede yaratacağımızı da eklememiz gerekiyor.
        Aktif sahneyi de EditorSceneManager.GetActiveScene() komutu ile alıyoruz.
        */
        GameObject yeniObje = PrefabUtility.InstantiatePrefab (obj as GameObject, EditorSceneManager.GetActiveScene ()) as GameObject;
        /*
        Eğer seçili bir obje varsa ve ayarlarımıda "Alt Obje olarak" oluşturma aktifse yeni objeyi 
        seçili olanın alt objesi - child haline getiriyoruz.
        */
        if (Selection.activeGameObject != null && altObjeOlarakEkle) {
            yeniObje.transform.parent = Selection.activeGameObject.transform;
        }
        /*
        Eğer yeni yaratılan objenin otomatik seçili hale gelmesi özelliğimiz aktif ise
        Selection.activeGameObject değişkenine atama yapıyoruz, bak bu obje aktif olsun diyoruz.
        */
        if (seciliHaldeOlustur) {
            Selection.activeGameObject = yeniObje;
        }

        /*
        Geri alma işlemini de yapabilmek amacıyla Undo yapısına yaptığımız işlemi kaydediyoruz.
        Burada RegisterCreatedObjectUndo komutu ile bir obje yarattığımızı ve bunun geri alınabilir
        hale gelmesini istediğimi belirtiyoruz.
        ilk değişken yarattığımız yeni obje, ikincisi ise Edit butonu altında göstereceğimiz yazı.
        İster CTRL-Z ile istersek edit altında bu yazıya sahip butona kaldırarak ekleme işlemini geri alabiliriz.

        */
        Undo.RegisterCreatedObjectUndo (yeniObje, "Yeni eklenen objeyi kaldır");

        //Obje yaratımı sonrasında paneli kapatmak amacıyla bu komutu ekledim. İsterseniz aktif halde bırakabilirsiniz.
        sahnePaneliniAc = false;
    }

    //Seçtiğimiz klasör altındaki klasörleri ve prefabların yollarını gerekli dizilere aktarmamızı sağlar.
    void PrefablariYukle () {
        //Eğer klasör seçili değilse hiçbir işlem yapmadan geri dönüyoruz.
        if (prefabKlasoru == "") {
            return;
        }
        //Varolan prefabların listesini temizliyoruz.
        klasorPrefablari.Clear ();

        /*
        Seçili klasörün alt klasörlerini elde ediyoruz.
        Bu haliyle yalnızca bir alttaki klasörlerin isimlerini alabiliyoruz.
        Ancak bütün prefablara erişebiliyoruz.
        */
        string[] klasorYollari = AssetDatabase.GetSubFolders (prefabKlasoru);
        altKlasorler = new string[klasorYollari.Length];

        //Klasörlerin isimlerii altKlasorler dizimize kaydediyoruz
        for (int i = 0; i < klasorYollari.Length; i++) {
            int ayirmaIndeksi = klasorYollari[i].LastIndexOf ('/');
            altKlasorler[i] = klasorYollari[i].Substring (ayirmaIndeksi + 1);
        }

        /*
        Bütün alt klasörleri dolanarak içerisindeki prefablara erişiyoruz
        ve klasorPrefablari dizimize bunların dosya yollarını ekliyoruz.
        */
        foreach (string klasor in klasorYollari) {
            List<string> gecici = new List<string> ();
            string[] altPrefablar = AssetDatabase.FindAssets ("t:prefab", new string[] { klasor });
            foreach (string prefabGUID in altPrefablar) {
                string prefabYolu = AssetDatabase.GUIDToAssetPath (prefabGUID);
                gecici.Add (prefabYolu);
            }
            klasorPrefablari.Add (gecici);
        }
    }

    //Klasör seçimi ekranını açmamızı sağlar.
    void PrefabSecmeEkrani () {
        /*
        OpenFolderPanel komutu ile dosya seçimi için dosya gezginini açabiliyoruz.
        Burada verdiğimiz ilk değişken olan "Prefab Klasörü" dosya gezgininde yazacak başlık,
        ikinci değişken klasör, son değişken ise seçilebilecek dosya tipi.
        Bu ekranda seçim yapıldığında bize string tipinde dosya yolu verisini iletecektir. 
        Bu veriyi geciciYol isimli değişkene atıyoruz.
        */
        string geciciYol = EditorUtility.OpenFolderPanel ("Prefab Klasörü", "", "folder");

        /*
        Assets klasörü altındaki dosyalara bakacağız. Bu yüzden IndexOf ve
        Substring ile filtreleme yaparak prefabKlasoru değişkenimizi guncelliyoruz.
        Eğer "/Assets/" içeren bir string verimiz yoksa, klasör farklı bir yerde seçildi demektir.
        Bu durumda string değeri "" hale getiriyoruz.
        Tabii ki, Assets ismine sahip alakasız bir yerdeki alt klasör de seçilebilir. Çok iyi bir 
        filtreleme yöntemi değil. Nasıl bir yol ile daha iyi hale getirebilirsiniz diye düşünmek
        iyi bir egzersiz olabilir :)
        */
        int index = geciciYol.IndexOf ("/Assets/");
        if (index >= 0) {
            prefabKlasoru = geciciYol.Substring (index + 1);
            PrefablariYukle ();
        } else {
            prefabKlasoru = "";
        }
    }

    //Verilen dosya yolundaki prefabın görseli elde etmemizi sağlar.
    Texture2D prefabGorseliAl (string prefabYolu) {
        Object obj = AssetDatabase.LoadAssetAtPath<GameObject> (prefabYolu);
        /*
        Burada asıl işi yapan GetAssetPreview komutu. Project panelindeki görselleri de bu kod sağlıyor aslında.
        Bizim de işimizi faslasıyla kolaylaştırıyor. Verilen objenin görselini önizleme bir görüntüsünü elde etmemizi sağlıyor.
        */
        return AssetPreview.GetAssetPreview (obj);
    }

}
