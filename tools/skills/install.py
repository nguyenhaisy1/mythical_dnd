#!/usr/bin/env python3
"""Link the six project-owned skills into the local discovery directory."""
import argparse
import os
from pathlib import Path
NAMES=('mythical-task','mythical-prototype','mythical-change-impact','mythical-kernel','mythical-content','mythical-validation')
def install(destination,check=False):
    source=Path(__file__).resolve().parent
    plans=[]
    for name in NAMES:
        src=source/name
        if not (src/'SKILL.md').is_file():raise ValueError(f'Missing skill: {src}')
        dst=destination/name
        matching=dst.is_symlink() and dst.resolve()==src.resolve()
        if (dst.exists() or dst.is_symlink()) and not matching:
            raise ValueError(f'Existing different skill, left untouched: {dst}')
        plans.append((src,dst,matching))
    if check:
        missing=[str(dst) for _,dst,matching in plans if not matching]
        if missing:raise ValueError('Missing links: '+', '.join(missing))
        print(f'All {len(plans)} skill links resolve to project sources.')
        return
    destination.mkdir(parents=True,exist_ok=True)
    for src,dst,matching in plans:
        if not matching:dst.symlink_to(src,target_is_directory=True)
        print(f'{dst.name}: {"already linked" if matching else "linked"}')
def main():
    default=Path(os.environ.get('CODEX_HOME') or Path.home()/'.codex')/'skills'
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--destination',type=Path,default=default)
    parser.add_argument('--check',action='store_true')
    args=parser.parse_args()
    try:install(args.destination.expanduser().absolute(),args.check)
    except (ValueError,OSError) as e:parser.exit(1,str(e)+'\n')
if __name__=='__main__':main()
