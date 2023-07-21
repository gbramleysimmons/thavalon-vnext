import React, { Component } from 'react';
import Typography from '@mui/joy/Typography';
import Input from '@mui/joy/Input';
import Button from '@mui/joy/Button';
class NewGame extends Component {
    constructor(props) {
        super(props);
        this.state = {players : [], playerFieldValue: "", ready: false, error: "", gameId: ""}
    }

    componentDidMount() {
        this.setState({ready: true});   
    }

    removePlayer(player) {
        let newState = this.state.players.filter(i => i !== player);
        this.setState({players : newState});
    }

    addPlayer(event)  {
        event.preventDefault();
        let player = this.state.playerFieldValue;
        let newState = this.state.players;
        newState.push(player);
        this.setState({ players: newState, playerFieldValue: "" });
    }

    updateInputValue(evt) {
        this.setState({ playerFieldValue: evt.target.value });
    }

    resetInput() {
        this.setState({ playerFieldValue: ""});

    }

    updateGameId(evt) {
        this.setState({ gameId: evt.target.value });
    }

    createGame() {
        const len = this.state.players.length;
        if (len != 5 && len != 7 && len != 8 && len != 10) {
            this.setState({ error: "invalid # of players" })
            console.log("Invalid # of players")
        }
        else {
            this.props.createGame(this.state.players)
        }
    }

    getGame(evt) {
        evt.preventDefault();
        this.props.getGame(this.state.gameId);
    }
    

    render() {
        let players = this.state.players.map(player => <li> {player} <button onClick={() => this.removePlayer(player)}>X</button></li>);

        return (
            <div>
                {this.state.ready ?  <div> 
                    <form onSubmit={(event) => this.addPlayer(event)}> <Input value={this.state.playerFieldValue} onChange={(evt) => this.updateInputValue(evt)}></Input> </form>
                    
                    <ul>{players}</ul>
                    <Button onClick={() => this.createGame()}> Go! </Button>

                    <form onSubmit={(event) => this.getGame(event)}> <Input value={this.state.gameId} onChange={(evt) => this.updateGameId(evt)}></Input> </form>
                    {this.state.error}
                </div> 
                 : <div> Loading... </div>}
            </div>
        );
    }

}
export default NewGame;
