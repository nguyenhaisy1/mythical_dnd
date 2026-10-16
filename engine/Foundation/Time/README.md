# Foundation/Time — FND-03

Portable clock-v1 đã triển khai: immutable WorldTick (Int64 không âm), positive WorkDuration và SimulationCalendar 12×30 ngày. Tick=phút, epoch ngày1/tháng1/năm1; checked overflow và invalid input trả TIME_RANGE. Default WorldTick=0 hợp lệ; default duration/date không hợp lệ và được guard khi sử dụng.

Không có mutable world clock, scheduler/AdvanceTime hoặc wall-clock dependency. Application và FND-10 sẽ dùng các primitive để thực hiện command/scheduler đúng owner. Xem docs/contracts/TIME-SCHEDULER-SPEC.md và docs/qa/SPRINT-01-EVIDENCE.md.
