"""Conservative source inventory for reproducible local document exports."""
import hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
def fingerprints():
    paths={ROOT/name for name in ['.agent','AGENTS.md','README.md','ARCHITECTURE.md','CONTRIBUTING.md','CHANGELOG.md','VERSION']}
    for folder in ['docs','rules','content','tools/docs','tools/checks','tools/skills']:
        paths.update(p for p in (ROOT/folder).rglob('*') if p.is_file() and p.suffix in {'.md','.csv','.json','.html','.py','.yaml','.yml'} and '__pycache__' not in p.parts)
    return [{'file':str(p.relative_to(ROOT)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted(paths)]
