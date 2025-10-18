using Microsoft.Extensions.DependencyInjection;

namespace PetTrack.ConsoleUI.UI;

public class Menu
{
    private readonly IServiceProvider _sp;
    public Menu(IServiceProvider sp) => _sp = sp;

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PetTrack ===");
            Console.WriteLine(" 1) Owner ekle");
            Console.WriteLine(" 2) Ownerları listele");
            Console.WriteLine(" 3) Owner güncelle");
            Console.WriteLine(" 4) Owner sil");
            Console.WriteLine(" 5) Pet ekle");
            Console.WriteLine(" 6) Petleri listele");
            Console.WriteLine(" 7) Pet güncelle");
            Console.WriteLine(" 8) Pet sil");
            Console.WriteLine(" 9) Cihaz bağla");
            Console.WriteLine("10) Günlük aktivite üret");
            Console.WriteLine("11) Uyarıları listele");
            Console.WriteLine("12) Günlük rapor: En çok yürüyenler");
            Console.WriteLine("13) En sağlıklı hayvanlar");
            Console.WriteLine("14) Kritik uyarıları e-posta simülasyonu");
            Console.WriteLine("15) Randevu çakışma kontrolü (demo)");
            Console.WriteLine("16) Çıkış");
            Console.WriteLine("17) Sağlık kaydı ekle");
            Console.WriteLine("18) Sağlık kayıtlarını listele");
            Console.WriteLine("19) Veteriner randevusu ekle");
            Console.WriteLine("20) Randevuları listele");
            Console.Write("Seçim: ");
            var input = Console.ReadLine()?.Trim();

            using var scope = _sp.CreateScope();
            var owners = scope.ServiceProvider.GetRequiredService<OwnerScreens>();
            var pets = scope.ServiceProvider.GetRequiredService<PetScreens>();
            var devices = scope.ServiceProvider.GetRequiredService<DeviceScreens>();
            var acts = scope.ServiceProvider.GetRequiredService<ActivityScreens>();
            var reports = scope.ServiceProvider.GetRequiredService<ReportsScreens>();
            var healthUi = scope.ServiceProvider.GetService<HealthRecordScreens>();
            var apptUi = scope.ServiceProvider.GetService<AppointmentScreens>();

            switch (input)
            {
                case "1": owners.AddOwner(); break;
                case "2": owners.ListOwners(); break;
                case "3": owners.UpdateOwner(); break;
                case "4": owners.DeleteOwner(); break;
                case "5": pets.AddPet(); break;
                case "6": pets.ListPets(); break;
                case "7": pets.UpdatePet(); break;
                case "8": pets.DeletePet(); break;
                case "9": devices.AttachDevice(); break;
                case "10": acts.GenerateToday(); break;
                case "11": acts.ListAlerts(); break;
                case "12": reports.TopWalkersDaily(); break;
                case "13": reports.HealthiestPets(); break;
                case "14": reports.SendCriticalAlertsEmailSimulation(); break;
                case "15": reports.CheckAppointmentConflictsDemo(); break;
                case "16": return;

                // yeni seçenekler
                case "17":
                    if (healthUi is null) { Info("HealthRecord ekranı kayıtlı değil."); break; }
                    healthUi.AddHealthRecord();
                    break;

                case "18":
                    if (healthUi is null) { Info("HealthRecord ekranı kayıtlı değil."); break; }
                    healthUi.ListHealthRecords();
                    break;

                case "19":
                    if (apptUi is null) { Info("Appointment ekranı kayıtlı değil."); break; }
                    apptUi.AddAppointment();
                    break;

                case "20":
                    if (apptUi is null) { Info("Appointment ekranı kayıtlı değil."); break; }
                    apptUi.ListAppointments();
                    break;

                default:
                    Info("Geçersiz seçim.");
                    break;
            }
        }
    }

    internal static void Info(string msg)
    {
        if (!string.IsNullOrWhiteSpace(msg))
            Console.WriteLine(msg);
        Console.WriteLine("Devam için Enter...");
        Console.ReadLine();
    }
}