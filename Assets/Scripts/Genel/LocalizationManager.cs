using System.Collections.Generic;

public static class LocalizationManager
{
    // Þimdilik metinleri burada tutuyoruz. Ýleride burasý JSON'dan okuyacak.
    private static Dictionary<string, string> tr_Metinler = new Dictionary<string, string>()
    {
        {"INTRO_1_1", "Adým Bekir. Savaþý geride býrakýp asýl mesleðine, beyaz önlüðüne geri dönmüþ eski bir askerim."},
        {"INTRO_1_2", "Hastane koridorlarý stresli derler... Ama emin olun; cephede kopan uzuvlarý dikmek çok daha aðýrdý."},
        {"INTRO_2_1", "Nöbetimin bitmesini bekleyen kendi halimde bir doktordum; ta ki eski silah arkadaþým Yavuz, kanlar içinde sedyeme düþene kadar."},
        {"INTRO_2_2", "Gözlerindeki korku her þeyi anlatýyordu. Sadece þunu sayýklayabildi: 'Gelecekler Bekir... Peþimdeler, beni burada býrakma!" },
        //{"INTRO_2_2", "Ne istiyosun bekir,  bu iþ nereye gidicek böyle \n kaç kere konuþmadýk mý , Uðurunu skiyem laðn" },
        {"INTRO_3_1", "Daha ne olduðunu soramadan hastaneyi cehenneme çevirdiler. Yavuz'u maskeli adamlar silah zoruyla çekip aldý. Haberler tek bir ismi iþaret ediyordu: Mafya."},
        {"INTRO_3_2", "Götürüldüðü yer 'Batak' dedikleri o karanlýk kuleydi. Her katýnda ayrý bir suçun iþlendiði, polisin bile kapýsýndan döndüðü bir suç yuvasý."},
        {"INTRO_4_1", "Eþim kollarýmda can verirken çaresizce izlemiþtim. Ama bu kez ayný hatayý yapmayacaðým. Dostumu o bataklýktan çekip alacaðým."},
        {"INTRO_4_2", "Eski bir askerin dostunu kaçýrmak onlarýn sonu olacak. Eðer oraya polis giremiyorsa, ben girerim!"}

    };

    public static string GetText(string key)
    {
        // Eðer anahtar varsa metni döndür, yoksa anahtarýn adýný döndür (hata görmemek için)
        return tr_Metinler.ContainsKey(key) ? tr_Metinler[key] : key;
    }
}