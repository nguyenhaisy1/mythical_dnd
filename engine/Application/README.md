# Game.Application

CommandPipeline/ thực thi TransferResource in-memory, kiểm envelope/auth/revision, validate proposals, atomically publish world+receipt+minimal transfer fact. Reject/noop chỉ metadata; fault injection trước/sau commit có tests. Scheduler/projections/WorldDirector/storage ports đầy đủ còn pending; không phải durable persistence. Xem docs/qa/SPRINT-02-EVIDENCE.md.
