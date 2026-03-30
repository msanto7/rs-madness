import { useEffect, useState } from 'react';
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import type { DragEndEvent } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy, sortableKeyboardCoordinates, arrayMove } from '@dnd-kit/sortable';
import { restrictToVerticalAxis } from '@dnd-kit/modifiers';
import SortableTeamRow from '../components/SortableTeamRow';
import apiClient from '../api/client';
import { getApiErrorMessages, getApiErrorStatus } from '../api/errors';

interface TeamRank {
  teamId: number;
  teamName: string;
  seed: number;
  region: string;
  rank: number;
}

interface EntryResponse {
  id: number;
  submittedAt: string | null;
  ranks: TeamRank[];
}

interface Team {
  id: number;
  name: string;
  seed: number;
  region: string;
}

function buildRanksSignature(ranks: TeamRank[]): string {
  return [...ranks]
    .sort((a, b) => a.teamId - b.teamId)
    .map((r) => `${r.teamId}:${r.rank}`)
    .join('|');
}

export default function RankingPage() {
  const [teams, setTeams] = useState<TeamRank[]>([]);
  const [submittedAt, setSubmittedAt] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [actionMessage, setActionMessage] = useState('');
  const [actionMessageType, setActionMessageType] = useState<'success' | 'error' | ''>('');
  const [hasSavedDraft, setHasSavedDraft] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [lastSavedSignature, setLastSavedSignature] = useState<string | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates })
  );

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    setActionMessage('');
    setActionMessageType('');

    setTeams((prev) => {
      const oldIndex = prev.findIndex((t) => t.teamId === active.id);
      const newIndex = prev.findIndex((t) => t.teamId === over.id);
      const reordered = arrayMove(prev, oldIndex, newIndex);
      // Reassign ranks based on new positions
      return reordered.map((t, i) => ({ ...t, rank: i + 1 }));
    });
  }

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      // Try to load existing entry first
      const entryRes = await apiClient.get<EntryResponse>('/bracketentry/me');
      const sortedRanks = [...entryRes.data.ranks].sort((a, b) => a.rank - b.rank);
      setSubmittedAt(entryRes.data.submittedAt);
      setTeams(sortedRanks);
      setHasSavedDraft(true);
      setLastSavedSignature(buildRanksSignature(sortedRanks));
    } catch (err: unknown) {
      if (getApiErrorStatus(err) === 404) {
        const teamsRes = await apiClient.get<Team[]>('/teams');
        setTeams(
          teamsRes.data.map((t, i) => ({
            teamId: t.id,
            teamName: t.name,
            seed: t.seed,
            region: t.region,
            rank: i + 1,
          }))
        );
        setHasSavedDraft(false);
        setLastSavedSignature(null);
      } else {
        setLoadError(getApiErrorMessages(err, 'Failed to load data.')[0]);
      }
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (loadError) return <p style={{ color: '#ef4444' }}>{loadError}</p>;

  const isLocked = submittedAt !== null;
  const currentSignature = buildRanksSignature(teams);
  const hasUnsavedChanges = !isLocked && (lastSavedSignature === null || currentSignature !== lastSavedSignature);
  const submitDisabled = !hasSavedDraft || isSaving || isSubmitting;
  const submitBtnStyle: React.CSSProperties = {
    ...btnStyle,
    background: submitDisabled ? '#94a3b8' : 'var(--orange)',
    borderColor: submitDisabled ? '#94a3b8' : 'var(--border)',
    color: submitDisabled ? '#e2e8f0' : '#0f172a',
    cursor: submitDisabled ? 'not-allowed' : 'pointer',
    opacity: submitDisabled ? 0.85 : 1,
  };

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '1rem',
        }}
      >
        <h1 style={{ margin: 0 }}>My Rankings</h1>
        {isLocked && (
          <span
            style={{
              background: 'var(--navy)',
              color: 'var(--orange)',
              padding: '0.3rem 0.75rem',
              borderRadius: '4px',
              fontSize: '0.85rem',
              fontWeight: 600,
            }}
          >
            Submitted
          </span>
        )}
      </div>

      {!isLocked && (
        <div
          style={{
            display: 'flex',
            gap: '0.75rem',
            marginBottom: '1rem',
          }}
        >
          <button onClick={handleSave} style={btnStyle} disabled={isSaving || isSubmitting}>
            {isSaving ? 'Saving...' : 'Save Draft'}
          </button>
          <button
            onClick={handleSubmit}
            style={submitBtnStyle}
            disabled={submitDisabled}
            title={!hasSavedDraft ? 'Save your bracket first to enable submit.' : undefined}
          >
            {isSubmitting ? 'Submitting...' : 'Submit Entry'}
          </button>
        </div>
      )}

      {actionMessage && (
        <p
          style={{
            marginTop: 0,
            marginBottom: '1rem',
            color: actionMessageType === 'error' ? '#ef4444' : '#16a34a',
            fontWeight: 600,
          }}
        >
          {actionMessage}
        </p>
      )}

      {!isLocked && (
        <p
          style={{
            marginTop: 0,
            marginBottom: '1rem',
            color: hasUnsavedChanges ? '#f59e0b' : '#16a34a',
            fontWeight: 600,
          }}
        >
          {hasUnsavedChanges ? 'Unsaved changes. Save draft to keep your latest ranking.' : 'All changes saved.'}
        </p>
      )}

      <div
        style={{
          display: 'flex',
          padding: '0.5rem 1rem',
          fontSize: '0.75rem',
          color: '#64748b',
          fontWeight: 600,
          textTransform: 'uppercase',
          letterSpacing: '0.5px',
        }}
      >
        <span style={{ width: '50px' }}>Rank</span>
        <span style={{ flex: 1 }}>Team</span>
        <span style={{ width: '60px', textAlign: 'center' }}>Seed</span>
        <span style={{ width: '100px', textAlign: 'center' }}>Region</span>
      </div>

      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
        modifiers={[restrictToVerticalAxis]}
      >
        <SortableContext items={teams.map((t) => t.teamId)} strategy={verticalListSortingStrategy}>
          {teams.map((team) => (
            <SortableTeamRow
              key={team.teamId}
              teamId={team.teamId}
              rank={team.rank}
              teamName={team.teamName}
              seed={team.seed}
              region={team.region}
              disabled={isLocked}
            />
          ))}
        </SortableContext>
      </DndContext>
    </div>
  );

  async function handleSave() {
    setIsSaving(true);
    try {
      setActionMessage('');
      setActionMessageType('');

      const payload = {
        ranks: teams.map((t) => ({ teamId: t.teamId, rank: t.rank })),
      };

      const res = await apiClient.put<EntryResponse>('/bracketentry/me/ranks', payload);
      const sortedRanks = res.data.ranks.sort((a, b) => a.rank - b.rank);
      setTeams(sortedRanks);
      setSubmittedAt(res.data.submittedAt);
      setHasSavedDraft(true);
      setLastSavedSignature(buildRanksSignature(sortedRanks));
      setActionMessage('Draft saved successfully.');
      setActionMessageType('success');
    } catch (err: unknown) {
      setActionMessage(getApiErrorMessages(err, 'Save failed.').join(' '));
      setActionMessageType('error');
    } finally {
      setIsSaving(false);
    }
  }

  async function handleSubmit() {
    if (!hasSavedDraft) return;

    setIsSubmitting(true);
    try {
      setActionMessage('');
      setActionMessageType('');

      const res = await apiClient.post<EntryResponse>('/bracketentry/me/submit');
      setTeams(res.data.ranks.sort((a, b) => a.rank - b.rank));
      setSubmittedAt(res.data.submittedAt);
      setActionMessage('Entry submitted successfully.');
      setActionMessageType('success');
    } catch (err: unknown) {
      setActionMessage(getApiErrorMessages(err, 'Submit failed.').join(' '));
      setActionMessageType('error');
    } finally {
      setIsSubmitting(false);
    }
  }
}

const btnStyle: React.CSSProperties = {
  background: 'var(--navy)',
  color: '#f3f4f6',
  border: '1px solid var(--border)',
  padding: '0.5rem 1.25rem',
  borderRadius: '6px',
  cursor: 'pointer',
  fontWeight: 600,
  fontSize: '0.85rem',
};
