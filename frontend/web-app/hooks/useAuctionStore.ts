import { Auction, PageResult } from "@/types"
import { create } from "zustand"

type State = {
    auctions: Auction[]
    totalCount: number
    pageCount: number
}

type Actions = {
    setData: (data: PageResult<Auction>) => void
    setCurrentPrice: (auctionId: string, amount: number) => void
}

const inititalState: State = {
    auctions: [],
    pageCount: 0,
    totalCount: 0
}

export const useAuctionStore = create<State & Actions>((set) => ({
    ...inititalState,
    setData: (data: PageResult<Auction>) => {
        set(() => ({
            auctions: data.results,
            totalCount: data.totalCount,
            pageCount: data.pageCount
        }))
    },
    setCurrentPrice: (auctionId: string, amount: number) => {
        set((state) => ({
            auctions: state.auctions.map((auction) => auction.id === auctionId
            ? {...auction, currentHighBid: amount} : auction)
        }))
    }
}));