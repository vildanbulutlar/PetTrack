using PetTrack.Domain.Common;

namespace PetTrack.Domain.Entities;
public class ActivityLog : BaseEntity
{
    public int DeviceId { get; set; }
    public TrackerDevice Device { get; set; } = default!;

    public DateTime Date { get; set; }      // UTC
    public int Steps { get; set; }          // adım
    public double Temperature { get; set; }// vücut sıcaklığı
}