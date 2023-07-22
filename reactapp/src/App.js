import React, { Component } from 'react';
import { withCookies } from 'react-cookie';
import { CssVarsProvider } from '@mui/joy/styles';
import Sheet from '@mui/joy/Sheet';
import Typography from '@mui/joy/Typography';
import NewGame from './NewGame';
import Game from './Game';

class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        const { cookies } = props;
        this.state = { game: null, loading: true, gameId: cookies.get("gameId") };
    }

    componentDidMount() {
        if (this.state.gameId) {
            this.getGame(this.state.gameId);
        }
    }


    static renderGame(game) {
        console.log(game.roles)
        return (
            <div>
                <h1>Game ID {game.gameId} </h1>
                <h2>Players</h2>
                <ul>
                    {
                        Object.entries(game.roles).sort(() => Math.random() - 0.5).map(([key, value]) => {
                            return <li>{value.roleName} | {value.alignment} | {value.playerName} | {value.information.map(ele => ele + "|")}</li>
                        })
                    }
                </ul>
            </div>
        );
    }

    render() {
        return (
            <CssVarsProvider>
                <Sheet sx={{ margin: "auto" , width: "90%", height: "90%", display: "flex", padding: "10px", flexDirection: "column"}} variant="outlined">
                    <Typography level="h4" component="h1"> Thavalon</Typography>
                    <div> 
                    {this.state.game ? <Game game={this.state.game} exitGame={this.exitGame} /> : <NewGame createGame={(players) => this.createGame(players)} getGame={(gameId) => this.getGame(gameId)} />}
                </div>
                </Sheet>
            </CssVarsProvider>
        );
    }

    exitGame = () => {
        this.clearCookies();
        this.setState({ game: null })
    }

    async createGame(players) {
        const response = await fetch('/creategame', { method: 'POST', headers: { "content-type": "application/json" }, body: JSON.stringify({ players: players }) });
        let data;
        try {
            data =  await response.json();
        } catch (e) {
            console.log(e);
            return;
    }   

        console.log(data);
        const { cookies } = this.props

        const gameId = data.gameId;
        cookies.set('gameId', data.gameId, { path: '/' });
        console.log("Create Game: GameId " + gameId)
        this.setState({ game: data, loading: false, gameId: gameId });
    }

    async getGame(gameId) {
        console.log("Get Game: GameId " + gameId)
        const response = await fetch("/"+ gameId, { method: 'GET' })
        const data = await response.json();
        console.log(data);
        this.setState({ game: data, loading: false });
    }

    clearCookies() {
        const { cookies } = this.props
        cookies.remove('gameId');
    }
}
export default withCookies(App);
