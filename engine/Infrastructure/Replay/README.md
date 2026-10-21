# Replay headless — QA only

ReplayHarness.Record thực thi ordered Execute/Drain actions và ghi checkpoints; Replay chỉ decode initial image rồi rerun processor, không apply after-images. Artifact principals là trusted QA input, không live auth. Manifest expected do host cung cấp trước execution; compiled registry không được chọn từ artifact.

Hash result/gameplay/integrity/journal/jobs bind versions/content/seed. Gameplay hash loại metadata receipts; integrity hash giữ toàn state. Divergence trả action index/category/revisions/hashes; chưa có full field diff (FND15). Unknown exceptions propagate; chỉ approved PipelineFailure codes thành checkpoint.

ReplayArtifactCodec dùng mdc1 canonical bounded64MiB/10000actions, rejects unsupported versions, invalid structure và unknown fields. Empty log có thể là no-op API, CLI golden bắt buộc nonempty. Không gắn production store hoặc side effect sinks.

Xem ADR016, replay-kernel golden và báo cáo Sprint7. RNG contexts/draw counters được giữ; transfer/scheduler chưa tiêu thụ gameplay draws.
