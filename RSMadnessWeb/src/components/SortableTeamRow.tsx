import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

interface Props {
  teamId: number;
  rank: number;
  teamName: string;
  seed: number;
  region: string;
  disabled: boolean;
}

export default function SortableTeamRow({ teamId, rank, teamName, seed, region, disabled }: Props) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: teamId, disabled });

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    display: 'flex',
    alignItems: 'center',
    padding: '0.6rem 1rem',
    background: isDragging ? 'var(--navy-light, #142380)' : 'var(--bg-surface)',
    borderRadius: '6px',
    marginBottom: '4px',
    border: isDragging ? '1px solid var(--orange)' : '1px solid var(--border)',
    opacity: isDragging ? 0.9 : 1,
    cursor: disabled ? 'default' : 'grab',
    zIndex: isDragging ? 10 : 'auto',
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      <span style={{
        width: '50px',
        fontWeight: 700,
        color: 'var(--orange)',
        fontSize: '0.9rem',
      }}>
        {rank}
      </span>
      <span style={{ flex: 1, color: 'var(--text-h)' }}>{teamName}</span>
      <span style={{ width: '60px', textAlign: 'center' }}>{seed}</span>
      <span style={{ width: '100px', textAlign: 'center', fontSize: '0.85rem' }}>
        {region}
      </span>
    </div>
  );
}
