<template>
  <main class="container">
    <h1>Users & Investments</h1>

    <section v-if="loading" class="status">
      <p>Loading…</p>
    </section>

    <section v-else-if="error" class="error">
      <p>{{ error }}</p>
    </section>

    <section v-else class="users">
      <article v-for="u in users" :key="u.id" class="user-card">
        <header class="user-header">
          <h2>User Id {{ u.id }}</h2>
        </header>

        <div v-if="investmentsError.get(u.id)" class="error">
          {{ investmentsError.get(u.id) }}
        </div>

        <div v-else class="investments">
          <div v-if="(investments.get(u.id) ?? []).length === 0" class="muted">
            No investments found.
          </div>

          <div v-for="inv in investments.get(u.id) ?? []"
               :key="inv.investmentId"
               class="investment-item">
            <div class="investment-summary">
              <strong>Id:</strong> {{ inv.investmentId }}
              <span class="spacer"></span>
              <strong>Name:</strong> {{ inv.name }}
            </div>

            <div v-if="detailsError.get(key(u.id, inv.investmentId))" class="error">
              {{ detailsError.get(key(u.id, inv.investmentId)) }}
            </div>

            <div v-else-if="!details.get(key(u.id, inv.investmentId))" class="status">
              Loading details…
            </div>

            <table v-else class="details-table">
              <caption>Investment Details</caption>
              <tbody>
                <tr>
                  <th scope="row">Number of Shares</th>
                  <td>{{ details.get(key(u.id, inv.investmentId))!.numberOfShares }}</td>
                </tr>
                <tr>
                  <th scope="row">Cost Basis / Share</th>
                  <td>{{ details.get(key(u.id, inv.investmentId))!.costBasisPerShare }}</td>
                </tr>
                <tr>
                  <th scope="row">Current Price</th>
                  <td>{{ details.get(key(u.id, inv.investmentId))!.currentPrice }}</td>
                </tr>
                <tr>
                  <th scope="row">Current Value</th>
                  <td>{{ details.get(key(u.id, inv.investmentId))!.currentValue }}</td>
                </tr>
                <tr>
                  <th scope="row">Term</th>
                  <td>
                    {{
                      details.get(key(u.id, inv.investmentId))!.termDisplay
                      ?? termLabel(details.get(key(u.id, inv.investmentId))!.term)
                    }}
                  </td>
                </tr>
                <tr>
                  <th scope="row">Total Gain/Loss</th>
                  <td>{{ details.get(key(u.id, inv.investmentId))!.totalGainLoss }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </article>
    </section>
  </main>
</template>

<script setup lang="ts">
  import { onMounted, ref } from "vue";

  /** API DTOs */
  type User = { id: number }; // per your endpoint that returns only IDs

  type InvestmentListItemDto = {
    investmentId: number;
    name: string;
  };

  type InvestmentTerm = "ShortTerm" | "LongTerm" | number;

  type InvestmentDetailsDto = {
    investmentId: number;
    name: string;
    numberOfShares: number;
    costBasisPerShare: number;
    currentPrice: number;
    currentValue: number;
    term: InvestmentTerm;
    termDisplay?: string;
    totalGainLoss: number;
  };

  /** State */
  const loading = ref < boolean > (true);
  const error = ref < string > ("");

  const users = ref < User[] > ([]);
  const investments = ref < Map < number, InvestmentListItemDto[]>> (new Map());
  const investmentsError = ref < Map < number, string>> (new Map());

  const details = ref < Map < string, InvestmentDetailsDto>> (new Map());
  const detailsError = ref < Map < string, string>> (new Map());

  /** Helpers */
  function key(userId: number, investmentId: number): string {
    return `${userId}:${investmentId}`;
  }

  function termLabel(term: InvestmentTerm): string {
    if (typeof term === "number") {
      return term === 1 ? "Long Term" : "Short Term";
    }
    return term === "LongTerm" ? "Long Term" : "Short Term";
  }

  /** Data loaders */
  async function fetchUsers(): Promise<void> {
    try {
      const resp = await fetch(`/api/users`);
      if (!resp.ok) {
        throw new Error(`Users HTTP ${resp.status}`);
      }
      const data = (await resp.json()) as User[];
      users.value = Array.isArray(data) ? data : [];
    }
    catch (e: unknown) {
      error.value = e instanceof Error ? e.message : "Failed to load users.";
    }
  }

  async function loadInvestmentsForUser(userId: number): Promise<void> {
    try {
      const resp = await fetch(`/api/users/${userId}/investments`);
      if (!resp.ok) {
        throw new Error(`Investments HTTP ${resp.status}`);
      }
      const data = (await resp.json()) as InvestmentListItemDto[];
      investments.value.set(userId, Array.isArray(data) ? data : []);
    }
    catch (e: unknown) {
      investmentsError.value.set(
        userId,
        e instanceof Error ? e.message : "Failed to load investments."
      );
    }
  }

  async function loadDetails(userId: number, investmentId: number): Promise<void> {
    const k = key(userId, investmentId);
    try {
      const resp = await fetch(`/api/users/${userId}/investments/${investmentId}`);
      if (!resp.ok) {
        throw new Error(`Details HTTP ${resp.status}`);
      }
      const data = (await resp.json()) as InvestmentDetailsDto;
      details.value.set(k, data);
    }
    catch (e: unknown) {
      detailsError.value.set(
        k,
        e instanceof Error ? e.message : "Failed to load details."
      );
    }
  }

  /** Orchestrate: users → investments → details (all visible at once) */
  onMounted(async () => {
    loading.value = true;
    error.value = "";
    await fetchUsers();

    // Load investments for all users in parallel
    await Promise.all(users.value.map(u => loadInvestmentsForUser(u.id)));

    // Load details for every investment for every user in parallel
    const allDetailCalls: Promise<void>[] = [];
    for (const u of users.value) {
      const invs = investments.value.get(u.id) ?? [];
      for (const inv of invs) {
        allDetailCalls.push(loadDetails(u.id, inv.investmentId));
      }
    }
    await Promise.all(allDetailCalls);

    loading.value = false;
  });
</script>

<style scoped>
  .container {
    max-width: 1024px;
    margin: 2rem auto;
    padding: 0 1rem;
    font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
  }

  h1 {
    margin-bottom: 1rem;
  }

  .status {
    margin: 0.5rem 0 1rem 0;
  }

  .error {
    color: #b00020;
  }

  .muted {
    opacity: 0.7;
  }

  .users {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .user-card {
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    padding: 1rem;
  }

  .user-header {
    margin-bottom: 0.5rem;
  }

  .investments {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  .investment-item {
    border: 1px solid #e5e7eb;
    border-radius: 10px;
    overflow: hidden;
  }

  .investment-summary {
    padding: 0.6rem 0.9rem;
    display: flex;
    align-items: center;
  }

    .investment-summary .spacer {
      display: inline-block;
      width: 1rem;
    }

  .details-table {
    width: 100%;
    border-collapse: collapse;
  }

    .details-table caption {
      text-align: left;
      font-weight: 600;
      margin: 0.5rem 0 0.25rem 0.5rem;
    }

    .details-table th,
    .details-table td {
      border: 1px solid #e5e7eb;
      padding: 0.5rem 0.6rem;
      vertical-align: top;
    }

    .details-table th {
      width: 40%;
      text-align: left;
      font-weight: 600;
    }
</style>
