window.sortableInterop = {
    instances: {},

    initMonsterSortable: function (containerElementId, dotNetHelper) {
        const el = document.getElementById(containerElementId);
        if (!el || this.instances[containerElementId]) return;

        let dragOrigin = null;

        const sortable = Sortable.create(el, {
            group: "monsters",
            animation: 150,
            handle: ".monster-drag-handle",
            onStart: function (evt) {
                dragOrigin = {
                    parent: evt.from,
                    nextSibling: evt.item.nextElementSibling
                };
            },
            onEnd: function (evt) {
                revertDomMove(evt.item, dragOrigin);
                dragOrigin = null;

                const monsterId = evt.item.getAttribute("data-monster-id");
                const targetBattleId = evt.to.getAttribute("data-battle-id");
                const newIndex = evt.newIndex;

                dotNetHelper.invokeMethodAsync("OnMonsterReordered", monsterId, targetBattleId, newIndex);
            }
        });

        this.instances[containerElementId] = sortable;
    },

    initBattleSortable: function (containerElementId, dotNetHelper) {
        const el = document.getElementById(containerElementId);
        if (!el || this.instances[containerElementId]) return;

        let dragOrigin = null;

        const sortable = Sortable.create(el, {
            animation: 150,
            handle: ".battle-drag-handle",
            onStart: function (evt) {
                dragOrigin = {
                    parent: evt.from,
                    nextSibling: evt.item.nextElementSibling
                };
            },
            onEnd: function (evt) {
                revertDomMove(evt.item, dragOrigin);
                dragOrigin = null;

                const battleId = evt.item.getAttribute("data-battle-id-value");
                const newIndex = evt.newIndex;

                dotNetHelper.invokeMethodAsync("OnBattleReordered", battleId, newIndex);
            }
        });

        this.instances[containerElementId] = sortable;
    },

    destroy: function (containerElementId) {
        const sortable = this.instances[containerElementId];
        if (sortable) {
            sortable.destroy();
            delete this.instances[containerElementId];
        }
    }
};

function revertDomMove(item, origin) {
    if (!origin) return;

    if (origin.nextSibling && origin.nextSibling.parentNode === origin.parent) {
        origin.parent.insertBefore(item, origin.nextSibling);
    } else {
        origin.parent.appendChild(item);
    }
}